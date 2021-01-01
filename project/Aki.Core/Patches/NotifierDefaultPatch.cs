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
	 * With this patch we force to use HTTP notifier on startup regardless of the game defaults.
	 * Targeted method is unique and only appears in GClass1328
	 */

	public class NotifierDefaultPatch : GenericPatch<NotifierDefaultPatch>
	{
		public NotifierDefaultPatch() : base(prefix: nameof(PatchPrefix)) { }

		public static Type __type;
		public static FieldInfo __channel;

		protected override MethodBase GetTargetMethod()
		{
			var methodName = "ApplyTransportType";
			var flags = BindingFlags.Public | BindingFlags.Instance;
			
			__type = PatcherConstants.TargetAssembly.GetTypes().Single(x => x.GetMethod(methodName, flags) != null);
			__channel = __type.GetField($"{typeof(GClass792).Name.ToLower()}_0", BindingFlags.NonPublic | BindingFlags.Instance);

			return __type.GetMethod(methodName, flags);
		}

		static bool PatchPrefix(ref object __instance, ENotificationTransportType transportType)
		{
			if (transportType == ENotificationTransportType.Default && __channel.GetValue(__instance) == null)
			{
				// game requires websock to run at least once before we can initialize HTTP notifier
				__type.GetMethod("ApplyTransportType").Invoke(__instance, new object[] { ENotificationTransportType.WebSocket });
			}

			return true;
		}
	}
}
