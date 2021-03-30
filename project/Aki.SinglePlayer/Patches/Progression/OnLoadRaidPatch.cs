/* OfflineLootPatch.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 * Martynas Gestautas
 */


using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using Aki.Common.Utils.App;
using Aki.Common.Utils.HTTP;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils;

namespace Aki.SinglePlayer.Patches.Progression
{
	public class OnLoadRaidPatch : GenericPatch<OnLoadRaidPatch>
	{
		public OnLoadRaidPatch() : base(prefix: nameof(PatchPostfix))
		{
        }

        protected override MethodBase GetTargetMethod()
        {
			return PatcherConstants.LocalGameType.BaseType.GetMethod("method_5", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		public static void PatchPostfix(string locationId)
		{
			SendLocationName(locationId);
			Config.WeaponDurabilityEnabled = GetDurabilityState();
		}

		private static void SendLocationName(string locationId)
		{
			new Request(Utils.Config.BackEndSession.GetPhpSessionId(), Config.BackendUrl).Send($"/raid/map/name?locationId={locationId}");
		}

		private static bool GetDurabilityState()
		{
			var json = new Request(null, Config.BackendUrl).GetJson("/singleplayer/settings/weapon/durability");

			if (string.IsNullOrWhiteSpace(json))
			{
				Debug.LogError("Aki.SinglePlayer: Received weapon durability state data is NULL, using fallback");
				return false;
			}

			Debug.LogError("Aki.SinglePlayer: Successfully received weapon durability state");
			return Convert.ToBoolean(json);
		}
	}
}
