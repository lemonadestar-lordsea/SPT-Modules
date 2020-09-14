using System.Reflection;
using EFT;
using SPTarkov.Common.Utils.Patching;
using SPTarkov.SinglePlayer.Utils;
using SPTarkov.SinglePlayer.Utils.Bots;
using BotDifficultyHandler = GClass280;

namespace SPTarkov.SinglePlayer.Patches.Bots
{
    public class BotDifficultyPatch : GenericPatch<BotDifficultyPatch>
    {
        public BotDifficultyPatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(BotDifficultyHandler).GetMethod("LoadDifficultyStringInternal", BindingFlags.Public | BindingFlags.Static);
        }

        private static bool PatchPrefix(ref string __result, BotDifficulty botDifficulty, WildSpawnType role)
        {
            foreach (Difficulty difficulty in Settings.Difficulties)
            {
                if (difficulty.Role == role && difficulty.BotDifficulty == botDifficulty)
                {
                    __result = difficulty.Json;
                }
            }

            return string.IsNullOrEmpty(__result);
        }
    }
}
