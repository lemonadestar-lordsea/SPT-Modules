using System.Reflection;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Utils;
using System.Linq;
using Aki.Reflection.Patching;

namespace Aki.SinglePlayer.Patches.Bots
{
	public class CoreDifficultyPatch : Patch
	{
		public CoreDifficultyPatch() : base(T: typeof(CoreDifficultyPatch), prefix: nameof(PatchPrefix))
		{
        }

        protected override MethodBase GetTargetMethod()
        {
			var methodName = "LoadCoreByString";
			var flags = BindingFlags.Public | BindingFlags.Static;

			return Constants.EftTypes.Single(x => x.GetMethod(methodName, flags) != null)
				.GetMethod(methodName, flags);
		}

		private static bool PatchPrefix(ref string __result)
		{
            __result = RequestHandler.GetJson("/singleplayer/settings/bot/difficulty/core/core");
			return string.IsNullOrWhiteSpace(__result);
        }
    }
}
