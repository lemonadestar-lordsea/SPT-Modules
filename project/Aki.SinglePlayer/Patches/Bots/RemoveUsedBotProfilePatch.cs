using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using EFT;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using BotData = GInterface17;

namespace Aki.SinglePlayer.Patches.Bots
{
    public class RemoveUsedBotProfilePatch : GenericPatch<RemoveUsedBotProfilePatch>
    {
        private static Type _targetInterface;
        private static Type _targetType;
        private static AccessTools.FieldRef<object, List<Profile>> _profilesField;

        static RemoveUsedBotProfilePatch()
        {
            _ = nameof(BotData.ChooseProfile);
        }

        public RemoveUsedBotProfilePatch() : base(prefix: nameof(PatchPrefix))
        {
            _targetInterface = Constants.EftTypes.Single(IsTargetInterface);
            _targetType = Constants.EftTypes.Single(IsTargetType);
            _profilesField = AccessTools.FieldRefAccess<List<Profile>>(_targetType, "list_0");
        }

        protected override MethodBase GetTargetMethod()
        {
            return _targetType.GetMethod("GetNewProfile", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        private static bool IsTargetInterface(Type type)
        {
            return type.IsInterface && type.GetProperty("StartProfilesLoaded") != null && type.GetMethod("CreateProfile") != null;
        }

        private bool IsTargetType(Type type)
        {
            return _targetInterface.IsAssignableFrom(type) && _targetInterface.IsAssignableFrom(type.BaseType);
        }

        private static bool PatchPrefix(ref Profile __result, object __instance, BotData data)
        {
            var profiles = _profilesField(__instance);

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
