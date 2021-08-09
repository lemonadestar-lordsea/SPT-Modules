using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Bots
{
    public class SpawnPmcPatch : Patch
    {
        private static Type _targetInterface;
        private static Type _targetType;
        private static FieldInfo _wildSpawnTypeField;
        private static FieldInfo _botDifficultyField;

        static SpawnPmcPatch()
        {
            _targetInterface = Constants.EftTypes.Single(IsTargetInterface);
            _targetType = Constants.EftTypes.Single(IsTargetType);
            _wildSpawnTypeField = _targetType.GetField("wildSpawnType_0", Constants.PrivateFlags);
            _botDifficultyField = _targetType.GetField("botDifficulty_0", Constants.PrivateFlags);
        }

        public SpawnPmcPatch() : base(T: typeof(SpawnPmcPatch), prefix: nameof(PatchPrefix))
        {
        }

        private static bool IsTargetInterface(Type type)
        {
            return type.IsInterface && type.GetMethod("ChooseProfile", new[] { typeof(List<Profile>), typeof(bool) }) != null;
        }

        private static bool IsTargetType(Type type)
        {
            if (!_targetInterface.IsAssignableFrom(type) || type.GetMethod("method_1", Constants.PrivateFlags) == null)
            {
                return false;
            }

            var fields = type.GetFields(Constants.PrivateFlags);
            return fields.Any(f => f.FieldType != typeof(WildSpawnType)) && fields.Any(f => f.FieldType == typeof(BotDifficulty));
        }

        protected override MethodBase GetTargetMethod()
        {
            return _targetType.GetMethod("method_1", Constants.PrivateFlags);
        }

        private static bool PatchPrefix(ref bool __result, object __instance, Profile x)
        {
            var botType = (WildSpawnType)_wildSpawnTypeField.GetValue(__instance);
            var botDifficulty = (BotDifficulty)_botDifficultyField.GetValue(__instance);

            __result = x.Info.Settings.Role == botType && x.Info.Settings.BotDifficulty == botDifficulty;
            return false;
        }
    }
}
