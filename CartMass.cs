using UnityEngine;

namespace Wagonborn
{
    internal static class CartMass
    {
        internal static float GetMultiplier(Player player)
        {
            float skillLevel = HaulingSkill.GetLevel(player);
            float skillCut = Mathf.Clamp01(PluginConfig.MaxSkillMassReduction.Value) * (skillLevel / 100f);
            return 1f - skillCut;
        }

        internal static int GetPullWeightPercent(Player player)
        {
            return Mathf.RoundToInt(GetMultiplier(player) * 100f);
        }
    }
}
