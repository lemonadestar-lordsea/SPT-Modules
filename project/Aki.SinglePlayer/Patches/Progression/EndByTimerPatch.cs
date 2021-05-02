using System;
using System.Linq;
using System.Reflection;
using EFT;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class EndByTimerPatch : GenericPatch<EndByTimerPatch>
    {
        private static PropertyInfo _profileIdProperty;
        private static MethodInfo _stopRaidMethod;

        static EndByTimerPatch()
        {
            _profileIdProperty = PatcherConstants.LocalGameType.BaseType
                .GetProperty("ProfileId", BindingFlags.NonPublic | BindingFlags.Instance)
                ?? throw new InvalidOperationException("'ProfileId' property not found");

            _stopRaidMethod = PatcherConstants.LocalGameType.BaseType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .SingleOrDefault(IsStopRaidMethod)
                ?? throw new InvalidOperationException("Method not found");
        }

        public EndByTimerPatch() : base(prefix: nameof(PrefixPatch))
        {
        }

        private static bool IsStopRaidMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();

            if (parameters.Length != 4
            || parameters[0].ParameterType != typeof(string)
            || parameters[0].Name != "profileId"
            || parameters[1].ParameterType != typeof(ExitStatus)
            || parameters[1].Name != "exitStatus"
            || parameters[2].ParameterType != typeof(string)
            || parameters[2].Name != "exitName"
            || parameters[3].ParameterType != typeof(float)
            || parameters[3].Name != "delay")
            {
                return false;
            }

            return true;
        }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.LocalGameType.BaseType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Single(x => x.Name.EndsWith("StopGame"));
        }

        private static bool PrefixPatch(object __instance)
        {
            var profileId = _profileIdProperty.GetValue(__instance) as string;
            var json = RequestHandler.GetJson("/singleplayer/settings/raid/endstate");
            var enabled = (string.IsNullOrWhiteSpace(json)) ? false : Convert.ToBoolean(json);

            if (!enabled)
            {
                return true;
            }

            _stopRaidMethod.Invoke(__instance, new object[] { profileId, ExitStatus.MissingInAction, null, 0f });
            return false;
        }
    }
}
