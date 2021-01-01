/* NotifierDefaultPatch.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using System;
using System.Linq;
using System.Reflection;
using Aki.Common.Utils.Patching;

namespace Aki.Core.Patches
{
	/* In patch 0.12.9.10510 they did a small refactor to ApplyTansportType to default to websocket.
	 * With this patch we always force to use HTTP notifier regardless of the user or game defaults.
	 * Targeted method is unique and only appears in GClass1328
	 */

	public class NotifierDefaultPatch : GenericPatch<NotifierDefaultPatch>
	{
		public NotifierDefaultPatch() : base(prefix: nameof(PatchPrefix)) { }

		public static Type __type;

		protected override MethodBase GetTargetMethod()
		{
			var methodName = "ApplyTransportType";
			var flags = BindingFlags.Public | BindingFlags.Instance;
			
			__type = PatcherConstants.TargetAssembly.GetTypes().Single(x => x.GetMethod(methodName, flags) != null);
			return __type.GetMethod(methodName, flags);
		}

		static bool PatchPrefix(ref object __instance, ENotificationTransportType transportType)
		{
			if (transportType != ENotificationTransportType.Default)
			{
				__type.GetMethod("ApplyTransportType").Invoke(__instance, new object[] { ENotificationTransportType.Default });
				return false;
			}

			return true;
		}
	}
}
