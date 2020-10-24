/* SpawnPmcPatch.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Martynas Gestautas
 * Merijn Hendriks
 */


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
            if (!fields.Any(f => f.FieldType == typeof(WildSpawnType)) || !fields.Any(f => f.FieldType == typeof(BotDifficulty)))
            {
                return false;
            }

            return true;
        }

        protected override MethodBase GetTargetMethod()
        {
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
