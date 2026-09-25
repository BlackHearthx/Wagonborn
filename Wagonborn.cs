using System.Reflection;
using BepInEx;
using HarmonyLib;
using Jotunn;
using Jotunn.Utils;

namespace Wagonborn
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Main.ModGuid)]
    [BepInDependency("zenox.teleporteverything", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("TastyChickenLegs.BetterCarts", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
    public class WagonbornPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.blackhearthx.wagonborn";
        public const string PluginName = "Wagonborn";
        public const string PluginVersion = "1.3.1";

        internal static WagonbornPlugin Instance { get; private set; }
        internal static Harmony Harmony { get; private set; }

        private void Awake()
        {
            Instance = this;
            PluginConfig.Bind(Config);
            ModLocalization.Register();
            HaulingSkill.Register();

            Harmony = new Harmony(PluginGUID);
            try
            {
                Harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (System.Exception ex)
            {
                Jotunn.Logger.LogError($"Harmony patch failed: {ex}");
            }

            Jotunn.Logger.LogInfo($"{PluginName} {PluginVersion} loaded ({PluginGUID})");
        }

        private void Update()
        {
            HaulingProgress.Tick();
            CartMapPin.Tick();
        }

        private void LateUpdate()
        {
            CartMapPin.LateTickUi();
        }

        private void OnDestroy()
        {
            Harmony?.UnpatchSelf();
        }
    }
}
