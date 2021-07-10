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
        private static Type _targetInterface;
        private static Type _targetType;
        private static AccessTools.FieldRef<object, WildSpawnType> _wildSpawnTypeField;
        private static AccessTools.FieldRef<object, BotDifficulty> _botDifficultyField;

        public SpawnPmcPatch() : base(prefix: nameof(PatchPrefix))
        {
            _targetInterface = PatcherConstants.EftTypes.Single(IsTargetInterface);
            _targetType = PatcherConstants.EftTypes.Single(IsTargetType);
            _wildSpawnTypeField = AccessTools.FieldRefAccess<WildSpawnType>(_targetType, "wildSpawnType_0");
            _botDifficultyField = AccessTools.FieldRefAccess<BotDifficulty>(_targetType, "botDifficulty_0");
        }

        private static bool IsTargetInterface(Type type)
        {
            return type.IsInterface && type.GetMethod("ChooseProfile", new[] { typeof(List<Profile>), typeof(bool) }) != null;
        }

        private bool IsTargetType(Type type)
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            if (!_targetInterface.IsAssignableFrom(type))
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
            return _targetType.GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static bool PatchPrefix(object __instance, ref bool __result, Profile x)
        {
            var botType = _wildSpawnTypeField(__instance);
            var botDifficulty = _botDifficultyField(__instance);

            __result = x.Info.Settings.Role == botType && x.Info.Settings.BotDifficulty == botDifficulty;

            return false;
        }
    }
}
