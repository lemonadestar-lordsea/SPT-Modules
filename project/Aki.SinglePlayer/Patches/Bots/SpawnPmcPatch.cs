using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;

namespace Aki.SinglePlayer.Patches.Bots
{
    public class SpawnPmcPatch : GenericPatch<SpawnPmcPatch>
    {
        private static BindingFlags _flags;
        private static Type _targetInterface;
        private static Type _targetType;
        private static FieldInfo _wildSpawnTypeField;
        private static FieldInfo _botDifficultyField;

        public SpawnPmcPatch() : base(prefix: nameof(PatchPrefix))
        {
            _flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            _targetInterface = Constants.EftTypes.Single(IsTargetInterface);
            _targetType = Constants.EftTypes.Single(IsTargetType);
            _wildSpawnTypeField = _targetType.GetField("wildSpawnType_0", _flags);
            _botDifficultyField = _targetType.GetField("botDifficulty_0", _flags);
        }

        private static bool IsTargetInterface(Type type)
        {
            return type.IsInterface && type.GetMethod("ChooseProfile", new[] { typeof(List<Profile>), typeof(bool) }) != null;
        }

        private bool IsTargetType(Type type)
        {
            if (!_targetInterface.IsAssignableFrom(type) || type.GetMethod("method_1", _flags) == null)
            {
                return false;
            }

            var fields = type.GetFields(_flags);
            return fields.Any(f => f.FieldType != typeof(WildSpawnType)) && fields.Any(f => f.FieldType == typeof(BotDifficulty));
        }

        protected override MethodBase GetTargetMethod()
        {
            return _targetType.GetMethod("method_1", _flags);
        }

        private static bool PatchPrefix(object __instance, ref bool __result, Profile x)
        {
            var botType = (WildSpawnType)_wildSpawnTypeField.GetValue(__instance);
            var botDifficulty = (BotDifficulty)_botDifficultyField.GetValue(__instance);

            __result = x.Info.Settings.Role == botType && x.Info.Settings.BotDifficulty == botDifficulty;
            return false;
        }
    }
}
