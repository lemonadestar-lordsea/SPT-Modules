using System.IO;
using System.Reflection;
using UnityEngine;
using EFT.UI;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;

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
			if (preloader == null)
			{
				preloader = Object.FindObjectOfType<PreloaderUI>();
			}

			if (preloader != null)
			{
				var raidID = Path.GetRandomFileName().Replace(".", string.Empty).Substring(0, 4).ToUpper();
				preloader.SetSessionId(raidID);
			}
		}
	}
}
