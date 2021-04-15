using System.Reflection;
using UnityEngine;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils;
using System.Linq;

namespace Aki.SinglePlayer.Patches.Bots
{
	public class CoreDifficultyPatch : GenericPatch<CoreDifficultyPatch>
	{
		public CoreDifficultyPatch() : base(prefix: nameof(PatchPrefix))
		{
        }

        protected override MethodBase GetTargetMethod()
        {
			var methodName = "LoadCoreByString";
			var flags = BindingFlags.Public | BindingFlags.Static;

			return PatcherConstants.TargetAssembly
                .GetTypes().Single(x => x.GetMethod(methodName, flags) != null)
                .GetMethod(methodName, flags);
		}

		public static bool PatchPrefix(ref string __result)
		{
            __result = RequestHandler.GetBotCoreDifficulty();
			return string.IsNullOrWhiteSpace(__result);
        }
    }
}
