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
			SetRaidID();
			SetWeaponDurability();
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

		private static void SetWeaponDurability()
		{
			var json = RequestHandler.GetJson("/singleplayer/settings/weapon/durability");
			var enabled = (string.IsNullOrWhiteSpace(json)) ? false : Convert.ToBoolean(json);

			Config.WeaponDurabilityEnabled = enabled;
		}
	}
}
