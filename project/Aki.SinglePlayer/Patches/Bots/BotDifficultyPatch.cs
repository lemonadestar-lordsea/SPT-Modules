using System.Reflection;
using System.Linq;
using EFT;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Utils;

namespace Aki.SinglePlayer.Patches.Bots
{
    public class BotDifficultyPatch : GenericPatch<BotDifficultyPatch>
    {
        public BotDifficultyPatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            var methodName = "LoadDifficultyStringInternal";
			var flags = BindingFlags.Public | BindingFlags.Static;

			return Constants.EftTypes.Single(x => x.GetMethod(methodName, flags) != null).GetMethod(methodName, flags);
        }

        private static bool PatchPrefix(ref string __result, BotDifficulty botDifficulty, WildSpawnType role)
        {
            __result = RequestHandler.GetJson($"/singleplayer/settings/bot/difficulty/{role.ToString()}/{botDifficulty.ToString()}");
            return string.IsNullOrWhiteSpace(__result);
        }
    }
}
