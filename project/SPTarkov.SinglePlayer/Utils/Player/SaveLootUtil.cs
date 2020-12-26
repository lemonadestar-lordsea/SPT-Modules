/* SaveLootUtil.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using EFT;
using Aki.Common.Utils.App;
using Aki.Common.Utils.HTTP;

namespace Aki.SinglePlayer.Utils.Player
{
	class SaveLootUtil
	{
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
			new Request(session, backendUrl).PutJson("/raid/profile/save", request.ToJson());
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
