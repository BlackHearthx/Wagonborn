using BepInEx.Configuration;
using UnityEngine;

namespace Wagonborn
{
    internal static class PluginConfig
    {
        internal static ConfigEntry<float> MaxSkillMassReduction;
        internal static ConfigEntry<float> XpPerSecond;
        internal static ConfigEntry<float> MinSpeedForXp;
        internal static ConfigEntry<float> WeightXpScale;
        internal static ConfigEntry<bool> EnableCartPortal;
        internal static ConfigEntry<float> CartSearchRadius;
        internal static ConfigEntry<bool> PreventCartAutoDetach;
        internal static ConfigEntry<float> AttachLeash;
        internal static ConfigEntry<bool> EnableQuickAttach;
        internal static ConfigEntry<KeyboardShortcut> AttachHotKey;
        internal static ConfigEntry<float> AttachDistance;
        internal static ConfigEntry<bool> AllowOutOfPlaceAttach;
        internal static ConfigEntry<bool> EnableBuddyHelp;
        internal static ConfigEntry<float> BuddyRange;
        internal static ConfigEntry<int> MaxBuddies;
        internal static ConfigEntry<float> BuddyMassReduction;
        internal static ConfigEntry<float> BuddyXpMultiplier;

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
                "While pulling, allow a longer leash before the cart auto-drops (see AttachLeash). Use V or Interact to unhitch anytime.");

            AttachLeash = config.Bind(
                "Portal",
                "AttachLeash",
                8f,
                new ConfigDescription(
                    "Max meters from the hitch before auto-detach when PreventCartAutoDetach is on (vanilla is ~2).",
                    new AcceptableValueRange<float>(3f, 20f)));

            EnableQuickAttach = config.Bind(
                "Crew",
                "EnableQuickAttach",
                true,
                "Hotkey attaches or detaches a nearby cart, even if you are a bit off the hitch.");

            AttachHotKey = config.Bind(
                "Crew",
                "AttachHotKey",
                new KeyboardShortcut(KeyCode.V),
                "Key to attach / detach a nearby cart.");

            AttachDistance = config.Bind(
                "Crew",
                "AttachDistance",
                3f,
                new ConfigDescription(
                    "Max meters from the hitch point for quick attach.",
                    new AcceptableValueRange<float>(1f, 5f)));

            AllowOutOfPlaceAttach = config.Bind(
                "Crew",
                "AllowOutOfPlaceAttach",
                true,
                "Allow attach when not perfectly lined up (uses AttachDistance).");

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
        }
    }
}
