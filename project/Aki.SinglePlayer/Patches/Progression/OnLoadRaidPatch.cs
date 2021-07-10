using System.IO;
using System.Reflection;
using UnityEngine;
using EFT.UI;
using Aki.Common.Utils.Patching;

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

		private static void PatchPostfix()
		{
			if (preloader == null)
			{
				preloader = GameObject.FindObjectOfType<PreloaderUI>();
			}

			if (preloader != null)
			{
				var raidID = Path.GetRandomFileName().Replace(".", string.Empty).Substring(0, 4).ToUpper();
				preloader.SetSessionId(raidID);
			}
		}
	}
}
