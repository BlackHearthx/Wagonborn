using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Wagonborn
{
    /// <summary>
    /// Live minimap/map pins for carts (Hearthwife pattern):
    /// save:false + m_pos every frame + m_pinUpdateRequired so the pin moves while you stand still.
    /// Every cart is on the map by default, even out of loaded range (last known ZDO position).
    /// Shift+Use hides or shows that cart for the local player only; the choice lives on the
    /// cart's ZDO under a per-player key and is written by the owner through an RPC, so nobody
    /// steals ownership (and the hitch) from whoever is pulling.
    /// </summary>
    [HarmonyPatch]
    internal static class CartMapPin
    {
        private const string HiddenKeyPrefix = "Wagonborn_PinHidden_";
        private const string RpcSetHidden = "Wagonborn_SetPinHidden";
        private const string CartPrefab = "Cart";
        private const float PinUiScale = 0.85f;
        private const float ResyncInterval = 0.5f;
        private const float PendingHold = 3f;

        private static readonly Dictionary<ZDOID, TrackedPin> Pins = new Dictionary<ZDOID, TrackedPin>();
        private static readonly Dictionary<ZDOID, Vagon> Loaded = new Dictionary<ZDOID, Vagon>();
        private static readonly HashSet<ZDOID> WorldCarts = new HashSet<ZDOID>();
        private static readonly HashSet<ZDOID> Wanted = new HashSet<ZDOID>();
        private static readonly List<ZDOID> ScratchRemove = new List<ZDOID>();
        private static readonly List<ZDO> ScanBuffer = new List<ZDO>();
        private static readonly Dictionary<ZDOID, Pending> PendingChoice = new Dictionary<ZDOID, Pending>();

        private static AccessTools.FieldRef<Minimap, bool> s_pinUpdateRequired;
        private static Sprite _wheelSprite;
        private static float _resyncAt;
        private static int _scanIndex;

        private sealed class TrackedPin
        {
            public Vagon Cart;
            public Minimap.PinData Pin;
            public Minimap Map;
        }

        private struct Pending
        {
            public bool Hidden;
            public float Until;
        }

        internal static bool IsEnabled()
        {
            return PluginConfig.EnableCartMapPin != null && PluginConfig.EnableCartMapPin.Value;
        }

        private static string HiddenKey(long playerId)
        {
            return HiddenKeyPrefix + playerId;
        }

        private static bool IsHiddenForMe(ZDO zdo)
        {
            Player local = Player.m_localPlayer;
            if (zdo == null || local == null)
            {
                return false;
            }

            if (PendingChoice.TryGetValue(zdo.m_uid, out Pending pending))
            {
                if (Time.time < pending.Until)
                {
                    return pending.Hidden;
                }

                PendingChoice.Remove(zdo.m_uid);
            }

            return zdo.GetBool(HiddenKey(local.GetPlayerID()), false);
        }

        internal static bool IsMarked(Vagon cart)
        {
            if (cart == null || cart.m_nview == null || !cart.m_nview.IsValid())
            {
                return false;
            }

            return !IsHiddenForMe(cart.m_nview.GetZDO());
        }

        internal static void ToggleMarked(Vagon cart)
        {
            Player local = Player.m_localPlayer;
            if (!IsEnabled() || cart == null || local == null ||
                cart.m_nview == null || !cart.m_nview.IsValid())
            {
                return;
            }

            bool nowMarked = !IsMarked(cart);
            ZDOID id = cart.m_nview.GetZDO().m_uid;
            PendingChoice[id] = new Pending { Hidden = !nowMarked, Until = Time.time + PendingHold };
            cart.m_nview.InvokeRPC(RpcSetHidden, local.GetPlayerID(), !nowMarked);
            _resyncAt = 0f;

            string key = nowMarked ? "$wagonborn_mappin_on" : "$wagonborn_mappin_off";
            string msg = Localization.instance != null
                ? Localization.instance.Localize(key)
                : (nowMarked ? "On the map" : "Off the map");
            local.Message(MessageHud.MessageType.Center, msg);
        }

        private static void RPC_SetHidden(Vagon cart, long playerId, bool hidden)
        {
            if (cart == null || cart.m_nview == null || !cart.m_nview.IsValid() || !cart.m_nview.IsOwner())
            {
                return;
            }

            ZDO zdo = cart.m_nview.GetZDO();
            string key = HiddenKey(playerId);
            if (hidden)
            {
                zdo.Set(key, true);
            }
            else if (zdo.GetBool(key, false))
            {
                zdo.Set(key, false);
            }
        }

        internal static void Tick()
        {
            if (!IsEnabled())
            {
                ClearAll();
                return;
            }

            Minimap map = Minimap.instance;
            if (map == null || ZDOMan.instance == null || Player.m_localPlayer == null)
            {
                ClearAll();
                return;
            }

            if (Time.time >= _resyncAt)
            {
                _resyncAt = Time.time + ResyncInterval;
                AdvanceWorldScan();
                Resync(map);
            }

            UpdatePinPositions(map);
        }

        internal static void LateTickUi()
        {
            if (!IsEnabled() || Pins.Count == 0)
            {
                return;
            }

            Minimap map = Minimap.instance;
            if (map == null)
            {
                return;
            }

            foreach (TrackedPin tracked in Pins.Values)
            {
                if (tracked?.Pin == null)
                {
                    continue;
                }

                ApplyWheelIcon(tracked.Pin);
                ApplyPinUiSize(tracked.Pin, map);
            }
        }

        /// <summary>
        /// Walks the local ZDO store a slice at a time for cart ZDOs, so carts parked
        /// far away keep a pin. A host sees the whole world; a client sees every cart
        /// it has been near this session, at its last synced position.
        /// </summary>
        private static void AdvanceWorldScan()
        {
            bool done;
            try
            {
                done = ZDOMan.instance.GetAllZDOsWithPrefabIterative(CartPrefab, ScanBuffer, ref _scanIndex);
            }
            catch
            {
                ScanBuffer.Clear();
                _scanIndex = 0;
                return;
            }

            if (!done)
            {
                return;
            }

            WorldCarts.Clear();
            for (int i = 0; i < ScanBuffer.Count; i++)
            {
                ZDO zdo = ScanBuffer[i];
                if (zdo != null && zdo.IsValid())
                {
                    WorldCarts.Add(zdo.m_uid);
                }
            }

            ScanBuffer.Clear();
            _scanIndex = 0;
        }

        private static void Resync(Minimap map)
        {
            Loaded.Clear();
            Wanted.Clear();

            List<Vagon> carts = Vagon.m_instances;
            for (int i = 0; i < carts.Count; i++)
            {
                Vagon cart = carts[i];
                if (cart == null || cart.m_nview == null || !cart.m_nview.IsValid())
                {
                    continue;
                }

                ZDO zdo = cart.m_nview.GetZDO();
                Loaded[zdo.m_uid] = cart;
                if (!IsHiddenForMe(zdo))
                {
                    Wanted.Add(zdo.m_uid);
                }
            }

            foreach (ZDOID id in WorldCarts)
            {
                if (Wanted.Contains(id) || Loaded.ContainsKey(id))
                {
                    continue;
                }

                ZDO zdo = ZDOMan.instance.GetZDO(id);
                if (zdo != null && zdo.IsValid() && !IsHiddenForMe(zdo))
                {
                    Wanted.Add(id);
                }
            }

            ScratchRemove.Clear();
            foreach (KeyValuePair<ZDOID, TrackedPin> pair in Pins)
            {
                if (!Wanted.Contains(pair.Key) || pair.Value.Map != map)
                {
                    ScratchRemove.Add(pair.Key);
                }
            }

            for (int i = 0; i < ScratchRemove.Count; i++)
            {
                RemoveTracked(ScratchRemove[i]);
            }

            foreach (ZDOID id in Wanted)
            {
                Loaded.TryGetValue(id, out Vagon cart);
                if (Pins.TryGetValue(id, out TrackedPin existing))
                {
                    existing.Cart = cart;
                    continue;
                }

                Vector3 pos;
                if (!TryGetPosition(id, cart, out pos))
                {
                    continue;
                }

                AddPin(id, cart, pos, map);
            }
        }

        private static bool TryGetPosition(ZDOID id, Vagon cart, out Vector3 pos)
        {
            if (cart != null && cart)
            {
                pos = cart.transform.position;
                return true;
            }

            ZDO zdo = ZDOMan.instance != null ? ZDOMan.instance.GetZDO(id) : null;
            if (zdo != null && zdo.IsValid())
            {
                pos = zdo.GetPosition();
                return true;
            }

            pos = Vector3.zero;
            return false;
        }

        private static void AddPin(ZDOID id, Vagon cart, Vector3 pos, Minimap map)
        {
            string name = Localization.instance != null
                ? Localization.instance.Localize("$wagonborn_mappin_name")
                : "Cart";

            try
            {
                Minimap.PinData pin = map.AddPin(
                    pos,
                    Minimap.PinType.Icon3,
                    name,
                    save: false,
                    isChecked: false,
                    0L);
                pin.m_doubleSize = false;
                ApplyWheelIcon(pin);
                ApplyPinUiSize(pin, map);
                RequestPinUiRefresh(map);

                Pins[id] = new TrackedPin
                {
                    Cart = cart,
                    Pin = pin,
                    Map = map
                };
            }
            catch
            {
            }
        }

        private static void UpdatePinPositions(Minimap map)
        {
            ScratchRemove.Clear();
            bool moved = false;
            foreach (KeyValuePair<ZDOID, TrackedPin> pair in Pins)
            {
                TrackedPin tracked = pair.Value;
                if (tracked?.Pin == null || tracked.Map != map)
                {
                    ScratchRemove.Add(pair.Key);
                    continue;
                }

                if (!TryGetPosition(pair.Key, tracked.Cart, out Vector3 pos))
                {
                    // ZDO gone: the cart was broken or removed.
                    ScratchRemove.Add(pair.Key);
                    continue;
                }

                if (tracked.Pin.m_pos != pos)
                {
                    tracked.Pin.m_pos = pos;
                    moved = true;
                }
            }

            for (int i = 0; i < ScratchRemove.Count; i++)
            {
                RemoveTracked(ScratchRemove[i]);
            }

            if (moved)
            {
                RequestPinUiRefresh(map);
            }
        }

        private static void RemoveTracked(ZDOID id)
        {
            if (!Pins.TryGetValue(id, out TrackedPin tracked))
            {
                return;
            }

            Pins.Remove(id);
            if (tracked?.Pin != null && Minimap.instance != null)
            {
                try
                {
                    Minimap.instance.RemovePin(tracked.Pin);
                    RequestPinUiRefresh(Minimap.instance);
                }
                catch
                {
                }
            }
        }

        private static void ClearAll()
        {
            if (Pins.Count == 0)
            {
                return;
            }

            ScratchRemove.Clear();
            foreach (ZDOID id in Pins.Keys)
            {
                ScratchRemove.Add(id);
            }

            for (int i = 0; i < ScratchRemove.Count; i++)
            {
                RemoveTracked(ScratchRemove[i]);
            }
        }

        private static void RequestPinUiRefresh(Minimap map)
        {
            if (map == null)
            {
                return;
            }

            if (s_pinUpdateRequired == null)
            {
                try
                {
                    s_pinUpdateRequired = AccessTools.FieldRefAccess<Minimap, bool>("m_pinUpdateRequired");
                }
                catch
                {
                    s_pinUpdateRequired = null;
                }
            }

            if (s_pinUpdateRequired != null)
            {
                s_pinUpdateRequired(map) = true;
            }
        }

        private static void ApplyWheelIcon(Minimap.PinData pin)
        {
            if (pin == null)
            {
                return;
            }

            Sprite wheel = GetOrCreateWheelSprite();
            if (wheel == null)
            {
                return;
            }

            pin.m_doubleSize = false;
            pin.m_icon = wheel;
            if (pin.m_iconElement != null)
            {
                pin.m_iconElement.sprite = wheel;
                pin.m_iconElement.color = Color.white;
            }
        }

        private static void ApplyPinUiSize(Minimap.PinData pin, Minimap map)
        {
            if (pin?.m_uiElement == null || map == null)
            {
                return;
            }

            float baseSize = map.m_mode == Minimap.MapMode.Large
                ? map.m_pinSizeLarge
                : map.m_pinSizeSmall;
            float size = baseSize * PinUiScale;
            pin.m_uiElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
            pin.m_uiElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
        }

        private static Sprite GetOrCreateWheelSprite()
        {
            if (_wheelSprite != null)
            {
                return _wheelSprite;
            }

            const int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            float cx = (size - 1) * 0.5f;
            float cy = (size - 1) * 0.5f;
            Color rim = new Color(0.78f, 0.52f, 0.22f, 1f);
            Color wood = new Color(0.55f, 0.34f, 0.14f, 1f);
            Color iron = new Color(0.62f, 0.62f, 0.66f, 1f);
            Color clear = Color.clear;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - cx;
                    float dy = y - cy;
                    float r = Mathf.Sqrt((dx * dx) + (dy * dy));
                    Color pixel = clear;

                    if (r < 14.5f && r > 11.5f)
                    {
                        pixel = rim;
                    }
                    else if (r <= 11.5f && r > 3.5f)
                    {
                        float ang = Mathf.Atan2(dy, dx);
                        float spoke = Mathf.Abs(Mathf.Sin(ang * 4f));
                        pixel = spoke > 0.72f ? wood : clear;
                    }

                    if (r <= 3.5f)
                    {
                        pixel = iron;
                    }

                    if (r <= 1.4f)
                    {
                        pixel = new Color(0.22f, 0.22f, 0.24f, 1f);
                    }

                    tex.SetPixel(x, y, pixel);
                }
            }

            tex.Apply(false, true);
            _wheelSprite = Sprite.Create(
                tex,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                100f);
            _wheelSprite.name = "Wagonborn_MapWheel";
            return _wheelSprite;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Vagon), "Awake")]
        private static void VagonAwakePostfix(Vagon __instance)
        {
            if (__instance == null || __instance.m_nview == null || __instance.m_nview.GetZDO() == null)
            {
                return;
            }

            Vagon cart = __instance;
            cart.m_nview.Register<long, bool>(RpcSetHidden,
                (sender, playerId, hidden) => RPC_SetHidden(cart, playerId, hidden));
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Vagon), nameof(Vagon.Interact))]
        private static bool InteractPrefix(Vagon __instance, Humanoid character, bool hold, bool alt)
        {
            if (!alt || hold || !IsEnabled() || __instance == null)
            {
                return true;
            }

            if (character != Player.m_localPlayer)
            {
                return true;
            }

            ToggleMarked(__instance);
            return false;
        }
    }
}
