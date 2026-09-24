using UnityEngine;

namespace Wagonborn
{
    /// <summary>
    /// Grants Hauling XP while the local player pulls — or helps push — a moving cart.
    /// </summary>
    internal static class HaulingProgress
    {
        private static Vagon _attached;
        private static Vagon _helping;
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
                _attached = null;
                _helping = null;
                return;
            }

            if (_attached == null || !_attached || !_attached.IsAttached(player))
            {
                _attached = null;
                if (Time.time >= _rescanAt)
                {
                    _rescanAt = Time.time + 0.5f;
                    _attached = FindAttachedCart(player);
                }
            }

            if (_attached != null)
            {
                _helping = null;
                GrantXp(player, _attached, 1f);
                return;
            }

            if (!PluginConfig.EnableBuddyHelp.Value)
            {
                _helping = null;
                return;
            }

            if (_helping == null || !_helping || !_helping.InUse() ||
                Vector3.Distance(player.transform.position, _helping.transform.position) >
                PluginConfig.BuddyRange.Value)
            {
                _helping = null;
                if (Time.time >= _rescanAt)
                {
                    _rescanAt = Time.time + 0.5f;
                    _helping = CartCrew.FindCartBeingHelped(player);
                }
            }

            if (_helping != null)
            {
                GrantXp(player, _helping, PluginConfig.BuddyXpMultiplier.Value);
            }
        }

        private static void GrantXp(Player player, Vagon cart, float xpScale)
        {
            Rigidbody body = cart.m_body != null
                ? cart.m_body
                : cart.GetComponent<Rigidbody>();
            if (body == null)
            {
                return;
            }

            if (body.linearVelocity.magnitude < PluginConfig.MinSpeedForXp.Value)
            {
                return;
            }

            float weightFactor = Mathf.Max(1f, body.mass / Mathf.Max(1f, PluginConfig.WeightXpScale.Value));
            float xp = PluginConfig.XpPerSecond.Value * Time.deltaTime * weightFactor * xpScale;
            player.RaiseSkill(HaulingSkill.SkillType, xp);
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
    }
}
