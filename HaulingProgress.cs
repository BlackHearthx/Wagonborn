using System.Collections.Generic;
using UnityEngine;

namespace Wagonborn
{
    /// <summary>
    /// Grants Hauling XP while the local player pulls — or helps push — a moving cart.
    /// Speed comes from the cart's position so helpers (non-owners) measure it the same way.
    /// </summary>
    internal static class HaulingProgress
    {
        private const float RescanInterval = 0.5f;
        private const float MaxPlausibleSpeed = 20f;

        private static Vagon _cart;
        private static bool _pulling;
        private static Vector3 _lastPos;
        private static float _speed;
        private static float _rescanAt;

        internal static void Tick()
        {
            if (HaulingSkill.SkillType == Skills.SkillType.None)
            {
                return;
            }

            Player player = Player.m_localPlayer;
            if (player == null)
            {
                SetCart(null, false);
                return;
            }

            if (!IsStillEarning(player))
            {
                if (Time.time < _rescanAt)
                {
                    return;
                }

                _rescanAt = Time.time + RescanInterval;
                Vagon pulled = FindAttachedCart(player);
                if (pulled != null)
                {
                    SetCart(pulled, true);
                }
                else
                {
                    SetCart(CartCrew.FindCartBeingHelped(player), false);
                }

                if (_cart == null)
                {
                    return;
                }
            }

            float dt = Time.deltaTime;
            if (dt <= 0f)
            {
                return;
            }

            Vector3 pos = _cart.transform.position;
            float instant = Vector3.Distance(pos, _lastPos) / dt;
            _lastPos = pos;

            // Portal jumps and sync snaps are not hauling.
            if (instant > MaxPlausibleSpeed)
            {
                instant = 0f;
            }

            _speed = Mathf.Lerp(_speed, instant, 0.2f);
            if (_speed < PluginConfig.MinSpeedForXp.Value)
            {
                return;
            }

            float load = CartLoadMass(_cart);
            float weightFactor = Mathf.Max(1f, load / Mathf.Max(1f, PluginConfig.LoadXpScale.Value));
            float scale = _pulling ? 1f : PluginConfig.BuddyXpMultiplier.Value;
            player.RaiseSkill(HaulingSkill.SkillType, PluginConfig.XpPerSecond.Value * dt * weightFactor * scale);
        }

        /// <summary>Vanilla UpdateMass formula, before any skill/buddy cut.</summary>
        internal static float CartLoadMass(Vagon cart)
        {
            float mass = cart.m_baseMass;
            Inventory inv = cart.m_container != null ? cart.m_container.GetInventory() : null;
            if (inv != null)
            {
                mass += inv.GetTotalWeight() * cart.m_itemWeightMassFactor;
            }

            return mass;
        }

        private static bool IsStillEarning(Player player)
        {
            if (_cart == null || !_cart)
            {
                return false;
            }

            if (_pulling)
            {
                return _cart.IsAttached(player);
            }

            return PluginConfig.EnableBuddyHelp.Value &&
                   _cart.IsAttached() &&
                   !_cart.IsAttached(player) &&
                   Vector3.Distance(player.transform.position, _cart.transform.position) <=
                   PluginConfig.BuddyRange.Value;
        }

        private static void SetCart(Vagon cart, bool pulling)
        {
            if (cart != _cart)
            {
                _speed = 0f;
                if (cart != null)
                {
                    _lastPos = cart.transform.position;
                }
            }

            _cart = cart;
            _pulling = pulling;
        }

        private static Vagon FindAttachedCart(Player player)
        {
            List<Vagon> carts = Vagon.m_instances;
            for (int i = 0; i < carts.Count; i++)
            {
                Vagon cart = carts[i];
                if (cart != null && cart.IsAttached(player))
                {
                    return cart;
                }
            }

            return null;
        }
    }
}
