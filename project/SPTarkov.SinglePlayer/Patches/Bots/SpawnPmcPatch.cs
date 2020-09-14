using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using EFT;
using SPTarkov.Common.Utils.Patching;

namespace SPTarkov.SinglePlayer.Patches.Bots
{
    public class SpawnPmcPatch : GenericPatch<SpawnPmcPatch>
    {
        private static Type targetInterface;
        private static Type targetType;

        private static AccessTools.FieldRef<object, WildSpawnType> wildSpawnTypeField;
        private static AccessTools.FieldRef<object, BotDifficulty> botDifficultyField;

        public SpawnPmcPatch() : base(prefix: nameof(PatchPrefix))
        {
            targetInterface = PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetInterface);
            targetType = PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType);
            wildSpawnTypeField = AccessTools.FieldRefAccess<WildSpawnType>(targetType, "Type");
            botDifficultyField = AccessTools.FieldRefAccess<BotDifficulty>(targetType, "BotDifficulty");
        }

        private static bool IsTargetInterface(Type type)
        {
            if (!type.IsInterface || type.GetMethod("ChooseProfile", new[] { typeof(List<Profile>), typeof(bool) }) == null)
            {
                return false;
            }

            return true;
        }

        private bool IsTargetType(Type type)
        {
            if (!targetInterface.IsAssignableFrom(type))
            {
                return false;
            }

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (!fields.Any(x => x.FieldType == typeof(BotDifficulty) && x.Name == "BotDifficulty")
            || !fields.Any(x => x.FieldType == typeof(EPlayerSide) && x.Name == "Side")
            || !fields.Any(x => x.FieldType == typeof(WildSpawnType) && x.Name == "Type"))
            {
                return false;
            }

            return true;
        }

        protected override MethodBase GetTargetMethod()
        {
            return targetType.GetMethod("method_0", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static bool PatchPrefix(object __instance, ref bool __result, Profile x)
        {
            var botType = wildSpawnTypeField(__instance);
            var botDifficulty = botDifficultyField(__instance);

            __result = x.Info.Settings.Role == botType && x.Info.Settings.BotDifficulty == botDifficulty;

            return false;
        }
    }
}
