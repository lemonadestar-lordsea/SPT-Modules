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
using Aki.Common.Utils;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils;

namespace Aki.SinglePlayer.Patches.Progression
{
	public class OnLoadRaidPatch : GenericPatch<OnLoadRaidPatch>
	{
		public OnLoadRaidPatch() : base(postfix: nameof(PatchPostfix))
		{
        }

        protected override MethodBase GetTargetMethod()
        {
			return PatcherConstants.LocalGameType.BaseType.GetMethod("method_5", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		public static void PatchPostfix(string locationId)
		{
			Config.WeaponDurabilityEnabled = RequestHandler.GetDurabilityState();
			RequestHandler.SendLocationName(locationId);
		}
	}
}
