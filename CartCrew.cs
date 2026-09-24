using System.Collections.Generic;
using BepInEx.Bootstrap;
using HarmonyLib;
using UnityEngine;

namespace Wagonborn
{
    /// <summary>
    /// Quick attach/detach hotkey + buddy mass help (nearby players lighten the load).
    /// Soft-skips when BetterCarts is loaded to avoid double hotkey / double mass cut.
    /// </summary>
    [HarmonyPatch]
    internal static class CartCrew
    {
        private const string BetterCartsGuid = "TastyChickenLegs.BetterCarts";

        private static bool? _betterCartsPresent;
        private static readonly List<Player> PlayerBuffer = new List<Player>();

        internal static bool OwnCrewActive()
        {
            if (_betterCartsPresent == null)
            {
                _betterCartsPresent =
                    Chainloader.PluginInfos != null &&
                    Chainloader.PluginInfos.ContainsKey(BetterCartsGuid);
                if (_betterCartsPresent == true)
                {
                    Jotunn.Logger.LogInfo(
                        "Wagonborn: BetterCarts detected — quick-attach and buddy mass deferred to it (Hauling XP for helpers still runs).");
                }
            }

            return _betterCartsPresent != true;
        }

        internal static void TickHotkey()
        {
            if (!OwnCrewActive() || !PluginConfig.EnableQuickAttach.Value)
            {
                return;
            }

            if (IgnoreKeyPresses() || !PluginConfig.AttachHotKey.Value.IsDown())
            {
                return;
            }

            Player player = Player.m_localPlayer;
            if (player == null)
            {
                return;
            }

            // Detach first: while pulling, the cart body is behind you and often
            // outside the hitch sphere — still unhitch that cart on the hotkey.
            Vagon attached = FindAttachedCart(player);
            if (attached != null)
            {
                attached.Interact(player, false, false);
                return;
            }

            float search = Mathf.Max(PluginConfig.AttachDistance.Value, 5f);
            Vagon cart = FindNearestToggleCart(player, search);
            if (cart != null)
            {
                cart.Interact(player, false, false);
            }
        }

        private static Vagon FindAttachedCart(Player player)
        {
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
                if (cart != null && cart.IsAttached(player))
                {
                    return cart;
                }
            }

            return null;
        }

        internal static int CountHelpers(Vagon cart)
        {
            if (cart == null || !PluginConfig.EnableBuddyHelp.Value)
            {
                return 0;
            }

            PlayerBuffer.Clear();
            Player.GetPlayersInRange(cart.transform.position, PluginConfig.BuddyRange.Value, PlayerBuffer);

            int helpers = 0;
            for (int i = 0; i < PlayerBuffer.Count; i++)
            {
                Player p = PlayerBuffer[i];
                if (p == null || cart.IsAttached(p))
                {
                    continue;
                }

                helpers++;
            }

            return Mathf.Min(helpers, PluginConfig.MaxBuddies.Value);
        }

        internal static float GetBuddyMassMultiplier(Vagon cart)
        {
            if (!OwnCrewActive() || !PluginConfig.EnableBuddyHelp.Value)
            {
                return 1f;
            }

            int helpers = CountHelpers(cart);
            if (helpers <= 0)
            {
                return 1f;
            }

            float cut = helpers * Mathf.Clamp01(PluginConfig.BuddyMassReduction.Value);
            return Mathf.Max(0.1f, 1f - cut);
        }

        /// <summary>
        /// Local player is helping (not pulling) a cart that is in use nearby.
        /// </summary>
        internal static Vagon FindCartBeingHelped(Player local)
        {
            if (local == null || !PluginConfig.EnableBuddyHelp.Value)
            {
                return null;
            }

            float range = PluginConfig.BuddyRange.Value;
            Vagon[] carts;
            try
            {
                carts = Object.FindObjectsByType<Vagon>(FindObjectsSortMode.None);
            }
            catch
            {
                carts = Object.FindObjectsOfType<Vagon>();
            }

            Vagon best = null;
            float bestDist = range;
            for (int i = 0; i < carts.Length; i++)
            {
                Vagon cart = carts[i];
                if (cart == null || !cart.InUse() || cart.IsAttached(local))
                {
                    continue;
                }

                float dist = Vector3.Distance(local.transform.position, cart.transform.position);
                if (dist <= bestDist)
                {
                    bestDist = dist;
                    best = cart;
                }
            }

            return best;
        }

        private static Vagon FindNearestToggleCart(Player player, float maxDist)
        {
            Vector3 origin = player.transform.position + Vector3.up;
            Collider[] hits = Physics.OverlapSphere(origin, maxDist);
            Vagon best = null;
            float bestDist = maxDist;

            for (int i = 0; i < hits.Length; i++)
            {
                Collider col = hits[i];
                if (col == null)
                {
                    continue;
                }

                Vagon cart = col.GetComponentInParent<Vagon>();
                if (cart == null || col.attachedRigidbody == null)
                {
                    continue;
                }

                if (!cart.IsAttached(player) && cart.InUse())
                {
                    continue;
                }

                float dist = Vector3.Distance(col.ClosestPoint(origin), origin);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = cart;
                }
            }

            return best;
        }

        private static bool IgnoreKeyPresses()
        {
            if (ZNetScene.instance == null || Player.m_localPlayer == null)
            {
                return true;
            }

            if (Minimap.IsOpen() || Console.IsVisible() || TextInput.IsVisible())
            {
                return true;
            }

            if (ZNet.instance != null && ZNet.instance.InPasswordDialog())
            {
                return true;
            }

            if (Chat.instance != null && Chat.instance.HasFocus())
            {
                return true;
            }

            if (StoreGui.IsVisible() || InventoryGui.IsVisible() || Menu.IsVisible())
            {
                return true;
            }

            if (TextViewer.instance != null && TextViewer.instance.IsVisible())
            {
                return true;
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Vagon), "CanAttach")]
        private static bool CanAttachPrefix(Vagon __instance, GameObject go, ref bool __result)
        {
            // Only relax the hitch check when grabbing on. While already attached,
            // vanilla (or PreventCartAutoDetach) must own the distance check —
            // otherwise the cart feels glued within AttachDistance.
            if (!OwnCrewActive() || !PluginConfig.EnableQuickAttach.Value ||
                !PluginConfig.AllowOutOfPlaceAttach.Value)
            {
                return true;
            }

            Player local = Player.m_localPlayer;
            if (local == null || go != local.gameObject || __instance == null ||
                __instance.m_attachPoint == null)
            {
                return true;
            }

            if (__instance.IsAttached(local) || __instance.IsAttached())
            {
                return true;
            }

            if (__instance.transform.up.y < 0.1f)
            {
                return true;
            }

            float dist = Vector3.Distance(
                go.transform.position + __instance.m_attachOffset,
                __instance.m_attachPoint.position);

            __result = !local.IsTeleporting() && !local.InDodge() &&
                       dist < PluginConfig.AttachDistance.Value;
            return false;
        }
    }
}
