using BepInEx.Configuration;

namespace Wagonborn
{
    internal static class PluginConfig
    {
        internal static ConfigEntry<float> MaxSkillMassReduction;
        internal static ConfigEntry<float> XpPerSecond;
        internal static ConfigEntry<float> MinSpeedForXp;
        internal static ConfigEntry<float> WeightXpScale;

        internal static void Bind(ConfigFile config)
        {
            MaxSkillMassReduction = config.Bind(
                "Hauling",
                "MaxSkillMassReduction",
                0.50f,
                "Cart mass reduction at Hauling 100 (0.50 = 50%). Hills still matter.");

            XpPerSecond = config.Bind(
                "Hauling",
                "XpPerSecond",
                0.35f,
                "Base Hauling XP per second while pulling a moving cart.");

            MinSpeedForXp = config.Bind(
                "Hauling",
                "MinSpeedForXp",
                0.6f,
                "Cart must move faster than this (m/s) to grant XP.");

            WeightXpScale = config.Bind(
                "Hauling",
                "WeightXpScale",
                80f,
                "Heavier carts grant more XP. XP is multiplied by mass / this value.");
        }
    }
}
