using BepInEx.Configuration;

namespace Wagonborn
{
    internal static class PluginConfig
    {
        internal static ConfigEntry<float> MaxSkillMassReduction;
        internal static ConfigEntry<float> XpPerSecond;
        internal static ConfigEntry<float> MinSpeedForXp;
        internal static ConfigEntry<float> LoadXpScale;
        internal static ConfigEntry<bool> EnableCartPortal;
        internal static ConfigEntry<float> CartSearchRadius;
        internal static ConfigEntry<bool> PreventCartAutoDetach;
        internal static ConfigEntry<float> AttachLeash;
        internal static ConfigEntry<float> AttachDistance;
        internal static ConfigEntry<bool> AllowOutOfPlaceAttach;
        internal static ConfigEntry<bool> EnableBuddyHelp;
        internal static ConfigEntry<float> BuddyRange;
        internal static ConfigEntry<int> MaxBuddies;
        internal static ConfigEntry<float> BuddyMassReduction;
        internal static ConfigEntry<float> BuddyXpMultiplier;
        internal static ConfigEntry<bool> EnableCartMapPin;

        internal static void Bind(ConfigFile config)
        {
            MaxSkillMassReduction = config.Bind(
                "Hauling",
                "MaxSkillMassReduction",
                0.50f,
                new ConfigDescription(
                    "Cart mass reduction at Hauling 100 (0.50 = 50%). Hills still matter.",
                    new AcceptableValueRange<float>(0f, 0.8f)));

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

            LoadXpScale = config.Bind(
                "Hauling",
                "LoadXpScale",
                240f,
                new ConfigDescription(
                    "A full cart teaches more. XP is multiplied by (cart + cargo weight) / this, never below 1. An empty cart weighs about 20.",
                    new AcceptableValueRange<float>(20f, 2000f)));

            EnableCartPortal = config.Bind(
                "Portal",
                "EnableCartPortal",
                true,
                "Attached cart teleports with you through portals and dungeon rune stones.");

            CartSearchRadius = config.Bind(
                "Portal",
                "CartSearchRadius",
                10f,
                new ConfigDescription(
                    "Meters around the teleport point to find your attached cart.",
                    new AcceptableValueRange<float>(1f, 30f)));

            PreventCartAutoDetach = config.Bind(
                "Portal",
                "PreventCartAutoDetach",
                true,
                "While pulling, allow a longer leash before the cart auto-drops (see AttachLeash). Unhitch with Use (E) anytime.");

            AttachLeash = config.Bind(
                "Portal",
                "AttachLeash",
                8f,
                new ConfigDescription(
                    "Max meters from the hitch before auto-detach when PreventCartAutoDetach is on (vanilla is ~2).",
                    new AcceptableValueRange<float>(3f, 20f)));

            AttachDistance = config.Bind(
                "Crew",
                "AttachDistance",
                3f,
                new ConfigDescription(
                    "Max meters from the hitch point when attaching with Use (E).",
                    new AcceptableValueRange<float>(1f, 5f)));

            AllowOutOfPlaceAttach = config.Bind(
                "Crew",
                "AllowOutOfPlaceAttach",
                true,
                "Allow attach with Use (E) when not perfectly lined up (uses AttachDistance).");

            EnableBuddyHelp = config.Bind(
                "Crew",
                "EnableBuddyHelp",
                true,
                "Nearby friends lighten a pulled cart. Helpers also earn Hauling XP while it moves.");

            BuddyRange = config.Bind(
                "Crew",
                "BuddyRange",
                5f,
                new ConfigDescription(
                    "Meters around the cart to count as helping.",
                    new AcceptableValueRange<float>(2f, 12f)));

            MaxBuddies = config.Bind(
                "Crew",
                "MaxBuddies",
                4,
                new ConfigDescription(
                    "Max helpers (not counting the puller) that reduce mass.",
                    new AcceptableValueRange<int>(1, 8)));

            BuddyMassReduction = config.Bind(
                "Crew",
                "BuddyMassReduction",
                0.15f,
                new ConfigDescription(
                    "Mass cut per helper (0.15 = 15% each). Stacks up to MaxBuddies.",
                    new AcceptableValueRange<float>(0.05f, 0.40f)));

            BuddyXpMultiplier = config.Bind(
                "Crew",
                "BuddyXpMultiplier",
                1f,
                new ConfigDescription(
                    "Hauling XP rate for helpers vs the puller (1 = same rate).",
                    new AcceptableValueRange<float>(0.25f, 1.5f)));

            EnableCartMapPin = config.Bind(
                "Map",
                "EnableCartMapPin",
                true,
                "Every cart shows a live map pin by default. Shift+E on a cart hides or shows that one, just for you.");
        }
    }
}
