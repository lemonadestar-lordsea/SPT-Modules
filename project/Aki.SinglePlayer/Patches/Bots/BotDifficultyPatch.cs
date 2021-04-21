using System.Reflection;
using System.Linq;
using EFT;
using Aki.Common.Utils.Patching;
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

			return PatcherConstants.TargetAssembly
                .GetTypes().Single(x => x.GetMethod(methodName, flags) != null)
                .GetMethod(methodName, flags);
        }

        private static bool PatchPrefix(ref string __result, BotDifficulty botDifficulty, WildSpawnType role)
        {
            __result = RequestHandler.GetBotDifficulty(role, botDifficulty);
            return string.IsNullOrWhiteSpace(__result);
        }
    }
}
