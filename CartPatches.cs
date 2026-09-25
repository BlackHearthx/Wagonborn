using HarmonyLib;
using UnityEngine;

namespace Wagonborn
{
    [HarmonyPatch(typeof(Vagon))]
    internal static class CartPatches
    {
        private static bool _strippedLegacy;

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Vagon.SetMass))]
        private static void SetMassPrefix(Vagon __instance, ref float mass)
        {
            if (__instance == null || __instance.m_nview == null || !__instance.m_nview.IsOwner())
            {
                return;
            }

            Player player = Player.m_localPlayer;
            if (player != null && __instance.IsAttached(player))
            {
                mass *= CartMass.GetMultiplier(player);
            }

            mass *= CartCrew.GetBuddyMassMultiplier(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Vagon.GetHoverText))]
        private static void GetHoverTextPostfix(Vagon __instance, ref string __result)
        {
            if (__instance == null || string.IsNullOrEmpty(__result))
            {
                return;
            }

            if (!_strippedLegacy)
            {
                StripOldVisuals(__instance);
                _strippedLegacy = true;
            }

            Player player = Player.m_localPlayer;
            string level = Mathf.FloorToInt(HaulingSkill.GetLevel(player)).ToString();
            string percent = CartMass.GetPullWeightPercent(player).ToString();

            string line;
            if (Localization.instance != null)
            {
                line = Localization.instance.Localize("$wagonborn_hover", level, percent);
            }
            else
            {
                line = "Hauling " + level + " — pull weight " + percent + "%";
            }

            __result += "\n" + line;

            if (CartPortal.IsActive() && player != null && __instance.IsAttached(player))
            {
                string portalHint = Localization.instance != null
                    ? Localization.instance.Localize("$wagonborn_cart_portal_hint")
                    : "Portals take the cart";
                __result += "\n" + portalHint;
            }

            if (PluginConfig.EnableBuddyHelp.Value)
            {
                string buddy = Localization.instance != null
                    ? Localization.instance.Localize("$wagonborn_buddy_hint")
                    : "Friends nearby lighten the load — and train Hauling";
                __result += "\n" + buddy;
            }

            if (CartMapPin.IsEnabled())
            {
                if (CartMapPin.IsMarked(__instance))
                {
                    string marked = Localization.instance != null
                        ? Localization.instance.Localize("$wagonborn_mappin_marked")
                        : "On the map";
                    __result += "\n" + marked;
                }

                string pinHint = Localization.instance != null
                    ? Localization.instance.Localize(
                        CartMapPin.IsMarked(__instance)
                            ? "$wagonborn_mappin_hint_hide"
                            : "$wagonborn_mappin_hint_show")
                    : "[$KEY_ALT + $KEY_USE] Map pin";
                __result += "\n" + pinHint;
            }
        }

        private static void StripOldVisuals(Vagon cart)
        {
            Transform visual = cart.transform.Find("Wagonborn_Visual");
            if (visual != null)
            {
                Object.Destroy(visual.gameObject);
            }

            Transform light = cart.transform.Find("Wagonborn_Light");
            if (light != null)
            {
                Object.Destroy(light.gameObject);
            }
        }
    }
}
