using System.Reflection;
using SPTarkov.Common.Utils.Patching;
using SPTarkov.SinglePlayer.Utils;
using BotDifficultyHandler = GClass280;

namespace SPTarkov.SinglePlayer.Patches.Bots
{
	public class CoreDifficultyPatch : GenericPatch<CoreDifficultyPatch>
	{
		public CoreDifficultyPatch() : base(prefix: nameof(PatchPrefix))
		{
        }

        protected override MethodBase GetTargetMethod()
        {
			return typeof(BotDifficultyHandler).GetMethod("LoadCoreByString", BindingFlags.Public | BindingFlags.Static);
		}

		public static bool PatchPrefix(ref string __result)
		{
			__result = Settings.CoreDifficulty;

			return string.IsNullOrEmpty(__result);
        }
    }
}
