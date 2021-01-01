/* NotifierDefaultPatch.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * GGaulin
 * Merijn Hendriks
 */


using System.Linq;
using System.Reflection;
using HarmonyLib;
using Aki.Common.Utils.Patching;
using NotifierManager = GClass1328;
using NotifierCallback = GClass792;

namespace Aki.Core.Patches
{
	/* In patch 0.12.9.10510 they did a small refactor to ApplyTansportType.
	 * It forced creation of websocket regardless of user choice for the default channel.
	 * With this patch we enable the game to respect user choice again.
	 * Targeted method is unique and only appears in GClass1328
	 */

	public class NotifierDefaultPatch : GenericPatch<NotifierDefaultPatch>
	{
		public NotifierDefaultPatch() : base(prefix: nameof(PatchPrefix)) { }

		static AccessTools.FieldRef<NotifierManager, NotifierCallback> callbackField = AccessTools.FieldRefAccess<NotifierManager, NotifierCallback>($"{typeof(GClass792).Name.ToLower()}_0");

		protected override MethodBase GetTargetMethod()
		{
			var methodName = "ApplyTransportType";
			return PatcherConstants.TargetAssembly.GetTypes().Single(x => x.GetMethod(methodName) != null).GetMethod(methodName);
		}

		static bool PatchPrefix(NotifierManager __instance)
		{
			var callback = new NotifierCallback();
			
			callback.url = "https://";
			callbackField(__instance) = callback;
			return true;
		}
	}
}
