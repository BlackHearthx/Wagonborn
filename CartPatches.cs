using HarmonyLib;
using UnityEngine;

namespace Wagonborn
{
    [HarmonyPatch(typeof(Vagon))]
    internal static class CartPatches
    {
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

        // Vanilla only re-weighs every 5 s; hitching and unhitching should feel instant.
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Vagon.AttachTo))]
        private static void AttachToPostfix(Vagon __instance)
        {
            RefreshMass(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Vagon.Detach))]
        private static void DetachPostfix(Vagon __instance)
        {
            RefreshMass(__instance);
        }

        private static void RefreshMass(Vagon cart)
        {
            if (cart == null || cart.m_nview == null || !cart.m_nview.IsValid() || !cart.m_nview.IsOwner())
            {
                return;
            }

            cart.UpdateMass();
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Vagon.GetHoverText))]
        private static void GetHoverTextPostfix(Vagon __instance, ref string __result)
        {
            if (__instance == null || string.IsNullOrEmpty(__result))
            {
                return;
            }

            Player player = Player.m_localPlayer;
            string level = Mathf.FloorToInt(HaulingSkill.GetLevel(player)).ToString();
            string percent = CartMass.GetPullWeightPercent(player).ToString();

            // Vanilla already Localize'd __result — append plain / already-localized lines only.
            // Do not leave $KEY_* tokens here (they will not expand in a Postfix).
            __result += "\n" + L("$wagonborn_hover", level, percent);

            if (CartPortal.IsActive() && player != null && __instance.IsAttached(player))
            {
                __result += "\n" + L("$wagonborn_cart_portal_hint");
            }

            int helpers = CartCrew.CountHelpers(__instance);
            if (helpers > 0)
            {
                __result += "\n" + L("$wagonborn_buddy_hover", helpers.ToString());
            }

            if (CartMapPin.IsEnabled())
            {
                __result += "\n" + (CartMapPin.IsMarked(__instance)
                    ? L("$wagonborn_mappin_hover_on")
                    : L("$wagonborn_mappin_hover_off"));
            }
        }

        private static string L(string key, params string[] args)
        {
            if (Localization.instance == null)
            {
                return key;
            }

            return args != null && args.Length > 0
                ? Localization.instance.Localize(key, args)
                : Localization.instance.Localize(key);
        }
    }
}
