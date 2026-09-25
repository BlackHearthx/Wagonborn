using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace Wagonborn
{
    /// <summary>
    /// Live minimap/map pins for carts (Hearthwife pattern):
    /// save:false + m_pos every frame + m_pinUpdateRequired so the pin moves while you stand still.
    /// Every loaded cart is on the map by default; Alt+Use hides or shows that cart only.
    /// </summary>
    [HarmonyPatch]
    internal static class CartMapPin
    {
        /// <summary>ZDO true = player hid this cart's pin. Absent/false = show (default).</summary>
        private const string HiddenKey = "Wagonborn_MapPinHidden";
        private const float PinUiScale = 0.85f;
        private const float RescanInterval = 0.5f;

        private static readonly Dictionary<ZDOID, TrackedPin> Pins = new Dictionary<ZDOID, TrackedPin>();
        private static AccessTools.FieldRef<Minimap, bool> s_pinUpdateRequired;
        private static Sprite _wheelSprite;
        private static float _rescanAt;
        private static readonly List<ZDOID> _scratchRemove = new List<ZDOID>();
        private static readonly HashSet<ZDOID> _wanted = new HashSet<ZDOID>();

        private sealed class TrackedPin
        {
            public Vagon Cart;
            public Minimap.PinData Pin;
            public Minimap Map;
        }

        internal static bool IsEnabled()
        {
            return PluginConfig.EnableCartMapPin != null && PluginConfig.EnableCartMapPin.Value;
        }

        internal static bool IsMarked(Vagon cart)
        {
            if (cart == null || cart.m_nview == null || !cart.m_nview.IsValid())
            {
                return false;
            }

            // Default on: only hide when the player opted this cart out.
            return !cart.m_nview.GetZDO().GetBool(HiddenKey, false);
        }

        internal static void SetMarked(Vagon cart, bool marked)
        {
            if (cart == null || cart.m_nview == null || !cart.m_nview.IsValid())
            {
                return;
            }

            if (!cart.m_nview.IsOwner())
            {
                cart.m_nview.ClaimOwnership();
            }

            cart.m_nview.GetZDO().Set(HiddenKey, !marked);
        }

        internal static void ToggleMarked(Vagon cart)
        {
            if (!IsEnabled() || cart == null)
            {
                return;
            }

            bool next = !IsMarked(cart);
            SetMarked(cart, next);

            Player local = Player.m_localPlayer;
            if (local == null)
            {
                return;
            }

            string key = next ? "$wagonborn_mappin_on" : "$wagonborn_mappin_off";
            string msg = Localization.instance != null
                ? Localization.instance.Localize(key)
                : (next ? "Cart marked on the map" : "Cart pin cleared");
            local.Message(MessageHud.MessageType.Center, msg);
        }

        internal static void Tick()
        {
            if (!IsEnabled())
            {
                ClearAll();
                return;
            }

            Minimap map = Minimap.instance;
            if (map == null)
            {
                ClearAll();
                return;
            }

            if (Time.time >= _rescanAt)
            {
                _rescanAt = Time.time + RescanInterval;
                ResyncWantedCarts(map);
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

        private static void ResyncWantedCarts(Minimap map)
        {
            _wanted.Clear();

            Vagon[] carts;
            try
            {
                carts = Object.FindObjectsByType<Vagon>(FindObjectsSortMode.None);
            }
            catch
            {
                carts = Object.FindObjectsOfType<Vagon>();
            }

            for (int i = 0; i < carts.Length; i++)
            {
                Vagon cart = carts[i];
                if (cart == null || cart.m_nview == null || !cart.m_nview.IsValid())
                {
                    continue;
                }

                if (!IsMarked(cart))
                {
                    continue;
                }

                ZDOID id = cart.m_nview.GetZDO().m_uid;
                _wanted.Add(id);
                EnsurePin(cart, id, map);
            }

            _scratchRemove.Clear();
            foreach (KeyValuePair<ZDOID, TrackedPin> pair in Pins)
            {
                if (!_wanted.Contains(pair.Key) ||
                    pair.Value.Cart == null ||
                    !pair.Value.Cart ||
                    pair.Value.Map != map)
                {
                    _scratchRemove.Add(pair.Key);
                }
            }

            for (int i = 0; i < _scratchRemove.Count; i++)
            {
                RemoveTracked(_scratchRemove[i]);
            }
        }

        private static void EnsurePin(Vagon cart, ZDOID id, Minimap map)
        {
            if (Pins.TryGetValue(id, out TrackedPin existing) &&
                existing.Pin != null &&
                existing.Map == map &&
                existing.Cart == cart)
            {
                return;
            }

            if (existing != null)
            {
                RemoveTracked(id);
            }

            string name = Localization.instance != null
                ? Localization.instance.Localize("$wagonborn_mappin_name")
                : "Cart";

            try
            {
                Minimap.PinData pin = map.AddPin(
                    cart.transform.position,
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
            _scratchRemove.Clear();
            foreach (KeyValuePair<ZDOID, TrackedPin> pair in Pins)
            {
                TrackedPin tracked = pair.Value;
                if (tracked?.Cart == null || !tracked.Cart || tracked.Pin == null)
                {
                    _scratchRemove.Add(pair.Key);
                    continue;
                }

                if (tracked.Map != map)
                {
                    _scratchRemove.Add(pair.Key);
                    continue;
                }

                Vector3 pos = tracked.Cart.transform.position;
                if (tracked.Pin.m_pos != pos)
                {
                    tracked.Pin.m_pos = pos;
                }

                RequestPinUiRefresh(map);
                ApplyWheelIcon(tracked.Pin);
            }

            for (int i = 0; i < _scratchRemove.Count; i++)
            {
                RemoveTracked(_scratchRemove[i]);
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

            _scratchRemove.Clear();
            foreach (ZDOID id in Pins.Keys)
            {
                _scratchRemove.Add(id);
            }

            for (int i = 0; i < _scratchRemove.Count; i++)
            {
                RemoveTracked(_scratchRemove[i]);
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
