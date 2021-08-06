using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT.UI;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class OnLoadRaidPatch : Patch
	{
		private static PreloaderUI _preloader;

		public OnLoadRaidPatch() : base(T: typeof(OnLoadRaidPatch), postfix: nameof(PatchPostfix))
		{
			_preloader = null;
        }

        protected override MethodBase GetTargetMethod()
        {
			return Constants.LocalGameType.BaseType.GetMethod("method_5", Constants.PrivateFlags);
		}

		private static void PatchPostfix()
		{
			if (_preloader == null)
			{
				_preloader = Object.FindObjectOfType<PreloaderUI>();
			}

			if (_preloader != null)
			{
				var raidID = Path.GetRandomFileName().Replace(".", string.Empty).Substring(0, 4).ToUpper();
				_preloader.SetSessionId(raidID);
			}
		}
	}
}
