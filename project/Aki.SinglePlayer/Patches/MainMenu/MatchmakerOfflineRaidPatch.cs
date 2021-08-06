using Aki.Common;
using Aki.Reflection.Patching;
using Aki.SinglePlayer.Models;
using Aki.SinglePlayer.Utils;
using EFT.UI;
using EFT.UI.Matchmaker;
using System.Reflection;
using UnityEngine;

namespace Aki.SinglePlayer.Patches.MainMenu
{
    public class MatchmakerOfflineRaidPatch : Patch
    {
        public MatchmakerOfflineRaidPatch() : base(T: typeof(MatchmakerOfflineRaidPatch), postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MatchmakerOfflineRaid).GetMethod(nameof(MatchmakerOfflineRaid.Show));
        }

        private static void PatchPostfix(UpdatableToggle ____offlineModeToggle,
            UpdatableToggle ____botsEnabledToggle,
            TMPDropDownBox ____aiAmountDropdown,
            TMPDropDownBox ____aiDifficultyDropdown,
            UpdatableToggle ____enableBosses,
            UpdatableToggle ____scavWars,
            UpdatableToggle ____taggedAndCursed)
        {
            // disable "no progression save" panel
            var warningPanel = GameObject.Find("Warning Panel");
            UnityEngine.Object.Destroy(warningPanel);

            // disable offline mode toggle
            ____offlineModeToggle.isOn = true;
            ____offlineModeToggle.gameObject.SetActive(false);

            // enable bots by defaults
            ____botsEnabledToggle.isOn = true;

            // get settings from server
            var json = RequestHandler.GetJson("/singleplayer/settings/raid/menu");
            var settings = Json.Deserialize<DefaultRaidSettings>(json);

            if (settings != null)
            {
                ____aiAmountDropdown.UpdateValue((int)settings.AiAmount, false);
                ____aiDifficultyDropdown.UpdateValue((int)settings.AiDifficulty, false);
                ____enableBosses.isOn = settings.BossEnabled;
                ____scavWars.isOn = settings.ScavWars;
                ____taggedAndCursed.isOn = settings.TaggedAndCursed;
            }
        }
    }
}
