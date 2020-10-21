using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using EFT;
using SPTarkov.Common.Utils.Patching;
using UnityEngine;

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
            wildSpawnTypeField = AccessTools.FieldRefAccess<WildSpawnType>(targetType, "wildSpawnType_0");
            botDifficultyField = AccessTools.FieldRefAccess<BotDifficulty>(targetType, "botDifficulty_0");
        }

        private static bool IsTargetInterface(Type type)
        {
            return type.IsInterface && type.GetMethod("ChooseProfile", new[] { typeof(List<Profile>), typeof(bool) }) != null;
        }

        private bool IsTargetType(Type type)
        {
            if (!targetInterface.IsAssignableFrom(type))
            {
                return false;
            }

            var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            if (!fields.Any(f => f.FieldType == typeof(WildSpawnType)))
            {
                return false;
            }
            Debug.LogError("Found WildSpawnType");

            if (!fields.Any(f => f.FieldType == typeof(BotDifficulty)))
            {
                return false;
            }
            Debug.LogError("Found BotDifficulty");

            return true;
        }

        protected override MethodBase GetTargetMethod()
        {
            Debug.LogError(String.Join(", ", targetType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Select(m => m.Name)));
            return targetType.GetMethod("method_0", BindingFlags.NonPublic | BindingFlags.Instance);
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
