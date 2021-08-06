using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotData = GInterface17;

namespace Aki.SinglePlayer.Patches.Bots
{
    public class RemoveUsedBotProfilePatch : Patch
    {
        private static BindingFlags _flags;
        private static Type _targetInterface;
        private static Type _targetType;
        private static FieldInfo _profilesField;

        static RemoveUsedBotProfilePatch()
        {
            _ = nameof(BotData.ChooseProfile);

            _flags = BindingFlags.Instance | BindingFlags.NonPublic;
            _targetInterface = Constants.EftTypes.Single(IsTargetInterface);
            _targetType = Constants.EftTypes.Single(IsTargetType);
            _profilesField = _targetType.GetField("list_0", _flags);
        }

        public RemoveUsedBotProfilePatch() : base(T: typeof(RemoveUsedBotProfilePatch), prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return _targetType.GetMethod("GetNewProfile", _flags);
        }

        private static bool IsTargetInterface(Type type)
        {
            return type.IsInterface && type.GetProperty("StartProfilesLoaded") != null && type.GetMethod("CreateProfile") != null;
        }

        private static bool IsTargetType(Type type)
        {
            return _targetInterface.IsAssignableFrom(type) && _targetInterface.IsAssignableFrom(type.BaseType);
        }

        private static bool PatchPrefix(ref Profile __result, object __instance, BotData data)
        {
            var profiles = (List<Profile>)_profilesField.GetValue(__instance);

            if (profiles.Count > 0)
            {
                // second parameter makes client remove used profiles
                __result = data.ChooseProfile(profiles, true);
            }
            else
            {
                __result = null;
            }

            return false;
        }
    }
}
