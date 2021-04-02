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
using Aki.Common.Utils.HTTP;
using Aki.SinglePlayer.Utils.Player;
using ClientMetrics = GClass1407;

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
            var isPlayerScav = false;
            var profile = session.Profile;

            if (___esideType_0 == ESideType.Savage)
            {
                profile = session.ProfileOfPet;
                isPlayerScav = true;
            }
            
            SaveProfileProgress(Utils.Config.BackendUrl, session.GetPhpSessionId(), result.Value0, profile, currentHealth, isPlayerScav);
        }

        public static void SaveProfileProgress(string backendUrl, string session, ExitStatus exitStatus, Profile profileData, PlayerHealth currentHealth, bool isPlayerScav)
		{
			SaveProfileRequest request = new SaveProfileRequest
			{
				exit = exitStatus.ToString().ToLower(),
				profile = profileData,
				health = currentHealth,
				isPlayerScav = isPlayerScav
			};

			// ToJson() uses an internal converter which prevents loops and do other internal things
			new Request(session, backendUrl).Send("/raid/profile/save", "PUT", request.ToJson(), true, false);
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
