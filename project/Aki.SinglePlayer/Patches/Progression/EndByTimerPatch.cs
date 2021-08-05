using System;
using System.Linq;
using System.Reflection;
using EFT;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Utils;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class EndByTimerPatch : Patch
    {
        private static Type _localGameBaseType;
        private static PropertyInfo _profileIdProperty;
        private static MethodInfo _stopRaidMethod;

        static EndByTimerPatch()
        {
            _localGameBaseType = Constants.LocalGameType.BaseType;
            _profileIdProperty = _localGameBaseType.GetProperty("ProfileId", Constants.PrivateFlags);
            _stopRaidMethod = _localGameBaseType.GetMethods(Constants.PrivateFlags).SingleOrDefault(IsStopRaidMethod);
        }

        public EndByTimerPatch() : base(T: typeof(EndByTimerPatch), prefix: nameof(PrefixPatch))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return _localGameBaseType.GetMethods(Constants.PrivateFlags).Single(x => x.Name.EndsWith("StopGame"));
        }

        private static bool IsStopRaidMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length == 4
                && parameters[0].Name == "profileId"
                && parameters[1].Name == "exitStatus"
                && parameters[2].Name == "exitName"
                && parameters[3].Name == "delay"
                && parameters[0].ParameterType == typeof(string)
                && parameters[1].ParameterType == typeof(ExitStatus)
                && parameters[2].ParameterType == typeof(string)
                && parameters[3].ParameterType == typeof(float));
        }

        private static bool PrefixPatch(object __instance)
        {
            var profileId = _profileIdProperty.GetValue(__instance) as string;
            var json = RequestHandler.GetJson("/singleplayer/settings/raid/endstate");
            var enabled = (!string.IsNullOrWhiteSpace(json)) ? Convert.ToBoolean(json) : false;

            if (!enabled)
            {
                return true;
            }

            _stopRaidMethod.Invoke(__instance, new object[] { profileId, ExitStatus.MissingInAction, null, 0f });
            return false;
        }
    }
}
