/* OfflineSavePatch.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 * Martynas Gestautas
 * Ginja
 */


using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils;
using Aki.SinglePlayer.Utils.Player;
using ClientMetrics = GClass1407;
using Aki.Common.Utils.App;

namespace Aki.SinglePlayer.Patches.Progression
{
    class OfflineSaveProfilePatch : GenericPatch<OfflineSaveProfilePatch>
    {
        public OfflineSaveProfilePatch() : base(prefix: nameof(PatchPrefix))
        {
            // compile-time check
            _ = nameof(ClientMetrics.Metrics);
        }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.MainApplicationType.GetMethod("method_41", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static void PatchPrefix(ESideType ___esideType_0, Result<ExitStatus, TimeSpan, ClientMetrics> result)
        {
            var currentHealth = Utils.Player.HealthListener.Instance.CurrentHealth;
            var session = Utils.Config.BackEndSession;
            var profileData = session.Profile;
            var isPlayerScav = false;

            if (___esideType_0 == ESideType.Savage)
            {
                profileData = session.ProfileOfPet;
                isPlayerScav = true;
            }

            SaveProfileRequest request = new SaveProfileRequest
			{
				exit = result.Value0.ToString().ToLower(),
				profile = profileData,
				health = currentHealth,
				isPlayerScav = isPlayerScav
			};

			RequestHandler.SaveLoot(Json.Serialize(request));
        }

		internal class SaveProfileRequest
		{
			public string exit = "left";
			public Profile profile;
			public bool isPlayerScav;
			public PlayerHealth health;
		}
    }
}
