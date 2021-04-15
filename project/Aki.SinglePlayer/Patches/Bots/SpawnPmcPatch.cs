using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using EFT;
using Aki.Common.Utils.Patching;

namespace Aki.SinglePlayer.Patches.Bots
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
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            if (!targetInterface.IsAssignableFrom(type))
            {
                return false;
            }

            if (type.GetMethod("method_1", flags) == null)
            {
                return false;
            }

            var fields = type.GetFields(flags);
            
            if (!fields.Any(f => f.FieldType == typeof(WildSpawnType)) || !fields.Any(f => f.FieldType == typeof(BotDifficulty)))
            {
                return false;
            }

            return true;
        }

        protected override MethodBase GetTargetMethod()
        {
            return targetType.GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance);
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
