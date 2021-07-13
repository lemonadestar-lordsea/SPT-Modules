using System;
using System.IO;
using System.Reflection;
using EFT.UI;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Utils;
using Aki.SinglePlayer.Utils.Progression;

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
			return Constants.LocalGameType.BaseType.GetMethod("method_5", Constants.PrivateFlags);
		}

		private static void PatchPostfix()
		{
			SetRaidID();
			SetWeaponDurability();
		}

		private static void SetRaidID()
		{
			if (preloader == null)
			{
				preloader = UnityEngine.Object.FindObjectOfType<PreloaderUI>();
			}

			if (preloader != null)
			{
				var raidID = Path.GetRandomFileName().Replace(".", string.Empty).Substring(0, 4).ToUpper();
				preloader.SetSessionId(raidID);
			}
		}

		private static void SetWeaponDurability()
		{
			var json = RequestHandler.GetJson("/singleplayer/settings/weapon/durability");
			var enabled = (string.IsNullOrWhiteSpace(json)) ? false : Convert.ToBoolean(json);

			DurabilityConfig.Enabled = enabled;
		}
	}
}
