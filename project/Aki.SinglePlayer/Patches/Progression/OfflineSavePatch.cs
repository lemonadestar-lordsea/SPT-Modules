using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Models;
using Aki.SinglePlayer.Utils;
using ClientMetrics = GClass1431;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class OfflineSaveProfilePatch : GenericPatch<OfflineSaveProfilePatch>
    {
        static OfflineSaveProfilePatch()
        {
            _ = nameof(ClientMetrics.Metrics);
        }

        public OfflineSaveProfilePatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.MainApplicationType.GetMethod("method_44", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        private static void PatchPrefix(ESideType ___esideType_0, Result<ExitStatus, TimeSpan, ClientMetrics> result)
        {
            var session = Utils.Config.BackEndSession;

            SaveProfileRequest request = new SaveProfileRequest
			{
				Exit = result.Value0.ToString().ToLower(),
				Profile = (___esideType_0 == ESideType.Savage) ? session.ProfileOfPet : session.Profile,
				Health = Utils.Healing.HealthListener.Instance.CurrentHealth,
				IsPlayerScav = (___esideType_0 == ESideType.Savage)
			};

			RequestHandler.PutJson("/raid/profile/save", request.ToJson());
        }
    }
}
