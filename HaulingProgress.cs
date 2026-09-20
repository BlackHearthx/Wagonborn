using UnityEngine;

namespace Wagonborn
{
    /// <summary>
    /// Grants Hauling XP while the local player pulls a moving cart.
    /// Caches the attached cart — no full-scene scan every frame.
    /// </summary>
    internal static class HaulingProgress
    {
        private static Vagon _attached;
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
                return;
            }

            if (_attached == null || !_attached || !_attached.IsAttached(player))
            {
                _attached = null;
                if (Time.time < _rescanAt)
                {
                    return;
                }

                _rescanAt = Time.time + 0.5f;
                _attached = FindAttachedCart(player);
                if (_attached == null)
                {
                    return;
                }
            }

            Rigidbody body = _attached.m_body != null
                ? _attached.m_body
                : _attached.GetComponent<Rigidbody>();
            if (body == null)
            {
                return;
            }

            if (body.linearVelocity.magnitude < PluginConfig.MinSpeedForXp.Value)
            {
                return;
            }

            float weightFactor = Mathf.Max(1f, body.mass / Mathf.Max(1f, PluginConfig.WeightXpScale.Value));
            float xp = PluginConfig.XpPerSecond.Value * Time.deltaTime * weightFactor;
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
