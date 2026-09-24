using System;
using System.Collections.Generic;
using BepInEx.Bootstrap;
using HarmonyLib;
using UnityEngine;

namespace Wagonborn
{
    /// <summary>
    /// Attached cart travels through portals / dungeon teleporters with the player.
    /// Flow adapted from Zenox TeleportEverything (cart-only): place on TeleportTo,
    /// final re-place + AttachTo when UpdateTeleport finishes.
    /// Skips entirely if TeleportEverything is loaded (avoids double transport).
    /// </summary>
    [HarmonyPatch]
    internal static class CartPortal
    {
        private const float ExitExtraDistance = 3f;
        private const string TeleportEverythingGuid = "zenox.teleporteverything";

        private static readonly Collider[] ColliderBuffer = new Collider[512];
        private static readonly Action<Vagon, GameObject> AttachTo =
            AccessTools.MethodDelegate<Action<Vagon, GameObject>>(
                AccessTools.Method(typeof(Vagon), "AttachTo"));
        private static readonly Func<Vagon, GameObject, bool> CanAttach =
            AccessTools.MethodDelegate<Func<Vagon, GameObject, bool>>(
                AccessTools.Method(typeof(Vagon), "CanAttach"));

        private static ZDOID? _pendingReattachCartId;
        private static bool? _teleportEverythingPresent;

        internal static bool IsActive()
        {
            if (!PluginConfig.EnableCartPortal.Value)
            {
                return false;
            }

            if (_teleportEverythingPresent == null)
            {
                _teleportEverythingPresent =
                    Chainloader.PluginInfos != null &&
                    Chainloader.PluginInfos.ContainsKey(TeleportEverythingGuid);
                if (_teleportEverythingPresent == true)
                {
                    Jotunn.Logger.LogInfo(
                        "Wagonborn: TeleportEverything detected — cart portal transport deferred to it.");
                }
            }

            return _teleportEverythingPresent != true;
        }

        private static Vagon FindAttachedCart(Vector3 pos, Character player)
        {
            int hits = Physics.OverlapSphereNonAlloc(pos, PluginConfig.CartSearchRadius.Value, ColliderBuffer);
            Vagon found = null;
            HashSet<Vagon> seen = new HashSet<Vagon>();

            for (int i = 0; i < hits; i++)
            {
                if (ColliderBuffer[i] == null)
                {
                    continue;
                }

                Vagon cart = ColliderBuffer[i].GetComponentInParent<Vagon>();
                if (cart == null || !seen.Add(cart))
                {
                    continue;
                }

                if (cart.IsAttached(player))
                {
                    found = cart;
                    break;
                }
            }

            return found;
        }

        private static Vagon FindCartByZdoid(Vector3 pos, ZDOID id)
        {
            int hits = Physics.OverlapSphereNonAlloc(pos, PluginConfig.CartSearchRadius.Value, ColliderBuffer);
            for (int i = 0; i < hits; i++)
            {
                if (ColliderBuffer[i] == null)
                {
                    continue;
                }

                Vagon cart = ColliderBuffer[i].GetComponentInParent<Vagon>();
                if (cart == null)
                {
                    continue;
                }

                ZNetView nview = cart.GetComponent<ZNetView>();
                if (nview != null && nview.IsValid() && nview.GetZDO().m_uid == id)
                {
                    return cart;
                }
            }

            return null;
        }

        private static void PlaceCartForAttach(Vagon cart, Vector3 referencePos, Quaternion referenceRot)
        {
            if (cart == null || cart.m_attachPoint == null)
            {
                return;
            }

            Vector3 attachLocal = Quaternion.Inverse(cart.transform.rotation) *
                                  (cart.m_attachPoint.position - cart.transform.position);
            Vector3 delta = referencePos + cart.m_attachOffset - referenceRot * attachLocal -
                            cart.transform.position;

            Rigidbody rootBody = cart.GetComponent<Rigidbody>();
            Rigidbody[] bodies = cart.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodies.Length; i++)
            {
                Rigidbody body = bodies[i];
                body.isKinematic = true;
                body.transform.position += delta;
                body.linearVelocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                if (body == rootBody)
                {
                    body.transform.rotation = referenceRot;
                }
            }

            cart.transform.rotation = referenceRot;
            Physics.SyncTransforms();
        }

        private static void TransportAttachedCart(Vector3 fromPos, Player player, Vector3 targetPos,
            Quaternion targetRot)
        {
            Vagon cart = FindAttachedCart(fromPos, player);
            if (cart == null)
            {
                return;
            }

            ZNetView nview = cart.GetComponent<ZNetView>();
            if (nview == null || !nview.IsValid())
            {
                return;
            }

            if (!nview.IsOwner())
            {
                nview.ClaimOwnership();
            }

            PlaceCartForAttach(cart, targetPos, targetRot);

            ZDO zdo = nview.GetZDO();
            if (zdo != null)
            {
                zdo.SetPosition(cart.transform.position);
            }

            Rigidbody[] bodies = cart.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodies.Length; i++)
            {
                bodies[i].isKinematic = true;
            }

            _pendingReattachCartId = zdo.m_uid;
            Jotunn.Logger.LogInfo(
                $"Wagonborn: cart '{cart.name}' teleported with player to {cart.transform.position}");

            MessageHud.instance?.ShowMessage(
                MessageHud.MessageType.TopLeft,
                Localization.instance != null
                    ? Localization.instance.Localize("$wagonborn_cart_portal")
                    : "Cart travels with you");
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), nameof(Player.TeleportTo))]
        private static void TeleportToClearancePrefix(Player __instance, ref Vector3 pos, Quaternion rot)
        {
            if (!IsActive() || __instance == null)
            {
                return;
            }

            if (FindAttachedCart(__instance.transform.position, __instance) == null)
            {
                return;
            }

            pos += rot * Vector3.forward * ExitExtraDistance;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), nameof(Player.TeleportTo))]
        private static void TeleportToTransportPostfix(Player __instance, Vector3 pos, Quaternion rot,
            bool __result)
        {
            if (!__result || !IsActive() || __instance == null)
            {
                return;
            }

            TransportAttachedCart(__instance.transform.position, __instance, pos, rot);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "UpdateTeleport")]
        private static void UpdateTeleportReattachPostfix(Player __instance, bool ___m_teleporting)
        {
            if (___m_teleporting || !_pendingReattachCartId.HasValue || !IsActive() || __instance == null)
            {
                return;
            }

            ZDOID id = _pendingReattachCartId.Value;
            _pendingReattachCartId = null;

            Vagon cart = FindCartByZdoid(__instance.transform.position, id);
            if (cart == null)
            {
                Jotunn.Logger.LogInfo("Wagonborn: cart reattach skipped — cart not found after teleport.");
                return;
            }

            PlaceCartForAttach(cart, __instance.transform.position, __instance.transform.rotation);

            ZNetView nview = cart.GetComponent<ZNetView>();
            if (nview != null && nview.IsValid())
            {
                nview.GetZDO()?.SetPosition(cart.transform.position);
            }

            if (CanAttach != null && AttachTo != null && CanAttach(cart, __instance.gameObject))
            {
                AttachTo(cart, __instance.gameObject);
                Jotunn.Logger.LogInfo($"Wagonborn: re-attached cart '{cart.name}' after portal.");
            }
            else
            {
                Jotunn.Logger.LogInfo($"Wagonborn: could not re-attach cart '{cart.name}' (CanAttach failed).");
            }

            Rigidbody[] bodies = cart.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodies.Length; i++)
            {
                bodies[i].isKinematic = false;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Vagon), "Update")]
        private static void VagonUpdatePreventDetachPrefix(Vagon __instance, out float __state)
        {
            __state = __instance != null ? __instance.m_detachDistance : 0f;
            if (!IsActive() || !PluginConfig.PreventCartAutoDetach.Value || __instance == null)
            {
                return;
            }

            // Generous leash while pulling — not infinite, so a long walk still
            // drops the hitch if Interact/hotkey somehow fails.
            float leash = Mathf.Max(__state, PluginConfig.AttachLeash.Value);
            __instance.m_detachDistance = leash;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Vagon), "Update")]
        private static void VagonUpdatePreventDetachPostfix(Vagon __instance, float __state)
        {
            if (__instance != null)
            {
                __instance.m_detachDistance = __state;
            }
        }
    }
}
