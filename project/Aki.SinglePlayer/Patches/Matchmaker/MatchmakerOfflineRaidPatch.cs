/* MatchmakerOfflineRaidPatch.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 * Martynas Gestautas
 */


using System;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using EFT.UI;
using EFT.UI.Matchmaker;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils;

namespace Aki.SinglePlayer.Patches.Matchmaker
{
    class MatchmakerOfflineRaidPatch : GenericPatch<MatchmakerOfflineRaidPatch>
    {
        public MatchmakerOfflineRaidPatch() : base(postfix: nameof(PatchPostfix))
        {
        }

        public static void PatchPostfix(UpdatableToggle ____offlineModeToggle, UpdatableToggle ____botsEnabledToggle,
            TMPDropDownBox ____aiAmountDropdown, TMPDropDownBox ____aiDifficultyDropdown, UpdatableToggle ____enableBosses,
            UpdatableToggle ____scavWars, UpdatableToggle ____taggedAndCursed)
        {
            ____offlineModeToggle.isOn = true;
            ____offlineModeToggle.gameObject.SetActive(false);
            ____botsEnabledToggle.isOn = true;

            var defaultRaidSettings = RequestHandler.GetDefaultRaidSettings();

            if (defaultRaidSettings != null)
            {
                ____aiAmountDropdown.UpdateValue((int)defaultRaidSettings.AiAmount, false);
                ____aiDifficultyDropdown.UpdateValue((int)defaultRaidSettings.AiDifficulty, false);
                ____enableBosses.isOn = defaultRaidSettings.BossEnabled;
                ____scavWars.isOn = defaultRaidSettings.ScavWars;
                ____taggedAndCursed.isOn = defaultRaidSettings.TaggedAndCursed;
            }

            var warningPanel = GameObject.Find("Warning Panel");
            UnityEngine.Object.Destroy(warningPanel);
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MatchmakerOfflineRaid).GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}
