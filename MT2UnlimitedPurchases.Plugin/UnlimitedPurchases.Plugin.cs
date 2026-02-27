using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace MT2UnlimitedPurchases.Plugin
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ConfigEntry<bool>? unlimitedPurchases;
        public static ConfigEntry<bool>? freePurchases;

        internal static new ManualLogSource Logger = new(MyPluginInfo.PLUGIN_GUID);
        public void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            unlimitedPurchases = Config.Bind("General", "UnlimitedPurchases", true, "Purchase an item any number of times.");
            freePurchases = Config.Bind("General", "FreePurchases", false, "Purchases cost 0 gold.");

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
            if (Plugin.unlimitedPurchases == null || !Plugin.unlimitedPurchases.Value)
            {
                return; // If the config is not set or disabled, do nothing
            }
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

    [HarmonyPatch(typeof(SaveManager), "PurchaseReward")]
    public class NoPurchaseCost
    {
        public static bool Prefix(ref int value, SaveManager __instance)
        {
            if (Plugin.freePurchases == null || !Plugin.freePurchases.Value)
            {
                return true; // If the config is not set or disabled, continue with the original method
            }
            value = 0; // Set purchase cost to 0
            return true; // Continue with the original method
        }
    }
}
