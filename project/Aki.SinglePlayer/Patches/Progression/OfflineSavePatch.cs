using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Models;
using Aki.SinglePlayer.Utils;
using ClientMetrics = GClass1407;

namespace Aki.SinglePlayer.Patches.Progression
{
    class OfflineSaveProfilePatch : GenericPatch<OfflineSaveProfilePatch>
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
            return PatcherConstants.MainApplicationType.GetMethod("method_41", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static void PatchPrefix(ESideType ___esideType_0, Result<ExitStatus, TimeSpan, ClientMetrics> result)
        {
            var session = Utils.Config.BackEndSession;

            SaveProfileRequest request = new SaveProfileRequest
			{
				exit = result.Value0.ToString().ToLower(),
				profile = (___esideType_0 == ESideType.Savage) ? session.ProfileOfPet : session.Profile,
				health = Utils.Healing.HealthListener.Instance.CurrentHealth,
				isPlayerScav = (___esideType_0 == ESideType.Savage)
			};

			RequestHandler.PutJson("/raid/profile/save", request.ToJson());
        }
    }
}
