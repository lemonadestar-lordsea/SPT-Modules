using System.IO;
using System.Reflection;
using UnityEngine;
using EFT.UI;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils;

namespace Aki.SinglePlayer.Patches.Progression
{
	public class OnLoadRaidPatch : GenericPatch<OnLoadRaidPatch>
	{
		private static PreloaderUI preloader;

		public OnLoadRaidPatch() : base(postfix: nameof(PatchPostfix))
		{
			preloader = null;
        }

        protected override MethodBase GetTargetMethod()
        {
			return PatcherConstants.LocalGameType.BaseType.GetMethod("method_5", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		public static void PatchPostfix(string locationId)
		{
			Config.WeaponDurabilityEnabled = RequestHandler.GetDurabilityState();
			RequestHandler.SendLocationName(locationId);
			SetRaidID();
		}

		private static void SetRaidID()
		{
			if (preloader == null)
			{
				preloader = GameObject.FindObjectOfType<PreloaderUI>();
			}

			if (preloader != null)
			{
				var raidID = Path.GetRandomFileName().Replace(".", "").Substring(0, 4).ToUpper();
				preloader.SetSessionId(raidID);
			}
		}
	}
}
