﻿using System.Collections.Generic;
using System.Reflection;
using SPTarkov.Common.Utils.Patching;
using SPTarkov.SinglePlayer.Utils;
using WaveInfo = GClass897;
using BotsPresets = GClass334;

namespace SPTarkov.SinglePlayer.Patches.Bots
{
    public class BotTemplateLimitPatch : GenericPatch<BotTemplateLimitPatch>
    {
        public BotTemplateLimitPatch() : base(postfix: nameof(PatchPostfix))
        {
            // compile-time checks
            _ = nameof(BotsPresets.CreateProfile);
            _ = nameof(WaveInfo.Difficulty);
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(BotsPresets).GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static void PatchPostfix(List<WaveInfo> __result, List<WaveInfo> wavesProfiles, List<WaveInfo> delayed)
        {
            /*
                In short this method sums Limits by grouping wavesPropfiles collection by Role and Difficulty
                then in each group sets Limit to 30, the remainder is stored in "delayed" collection.
                So we change Limit of each group.
                Clear delayed waves, we don't need them if we have enough loaded profiles and in method_2 it creates a lot of garbage.
            */

            delayed?.Clear();
            
            foreach (WaveInfo wave in __result)
            {
				wave.Limit = Settings.Limits[wave.Role];
            }
        }
    }
}
