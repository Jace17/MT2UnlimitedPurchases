using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace MT2UnlimitedPurchases.Plugin
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new(MyPluginInfo.PLUGIN_GUID);
        public void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            // Uncomment if you need harmony patches, if you are writing your own custom effects.
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(MerchantGoodState), "ClaimReward")]
    public class UnlimitedPurchases
    {
        public static void Postfix(MerchantGoodState __instance, ref bool ___claimed, ref int ___remainingUses, int ___totalUses)
        {
            if (__instance.RewardData != null)
            {
                if (__instance.RewardData.name == "RerollMerchantRewardDataArtifactOnly"
                    || (__instance.RewardData.GetType().ToString() is "RelicRewardData"))
                {
                    return;
                }
            }
            ___claimed = false;
            ___remainingUses = ___totalUses;
        }
    }
}
