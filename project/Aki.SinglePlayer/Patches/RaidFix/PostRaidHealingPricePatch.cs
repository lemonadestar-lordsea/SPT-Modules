using System;
using System.Linq;
using System.Reflection;
using Aki.Reflection.Patching;
using EFT;
using HarmonyLib;

namespace Aki.SinglePlayer.Patches.RaidFix
{
    public class PostRaidHealingPricePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Profile).GetNestedTypes()
                .Single(x => x.GetMethod("UpdateLevel", BindingFlags.NonPublic | BindingFlags.Instance)?.IsVirtual ?? false)
                .GetMethod("UpdateLevel", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPrefix]
        protected static void PatchPrefix(Profile.GClass1466 __instance)
        {
            var loyaltyLevel = __instance.Settings.GetLoyaltyLevel(__instance);
            var loyaltyLevelSettings = __instance.Settings.GetLoyaltyLevelSettings(loyaltyLevel);

            if (loyaltyLevelSettings == null)
            {
                throw new IndexOutOfRangeException($"Loyalty level {loyaltyLevel} not found.");
            }

            Traverse.Create(__instance).Property("CurrentLoyalty").SetValue(loyaltyLevelSettings.Value);
        }
    }
}