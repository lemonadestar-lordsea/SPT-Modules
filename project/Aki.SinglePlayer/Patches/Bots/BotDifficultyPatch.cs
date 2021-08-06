using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Utils;
using EFT;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Bots
{
    public class BotDifficultyPatch : Patch
    {
        public BotDifficultyPatch() : base(T: typeof(BotDifficultyPatch), prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            var methodName = "LoadDifficultyStringInternal";
			var flags = BindingFlags.Public | BindingFlags.Static;

			return Constants.EftTypes.Single(x => x.GetMethod(methodName, flags) != null)
                .GetMethod(methodName, flags);
        }

        private static bool PatchPrefix(ref string __result, BotDifficulty botDifficulty, WildSpawnType role)
        {
            __result = RequestHandler.GetJson($"/singleplayer/settings/bot/difficulty/{role}/{botDifficulty}");
            return string.IsNullOrWhiteSpace(__result);
        }
    }
}
