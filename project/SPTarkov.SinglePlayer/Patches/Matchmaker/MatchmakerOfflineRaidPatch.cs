using System.Reflection;
using UnityEngine;
using EFT.UI;
using EFT.UI.Matchmaker;
using SPTarkov.Common.Utils.Patching;
using SPTarkov.SinglePlayer.Utils;
using SPTarkov.SinglePlayer.Utils.DefaultSettings;
using SPTarkov.Common.Utils.HTTP;
using Newtonsoft.Json;
using System;

namespace SPTarkov.SinglePlayer.Patches.Matchmaker
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

            DefaultRaidSettings defaultRaidSettings = Request();

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

        private static DefaultRaidSettings Request()
        {
            var json = new Request(null, Config.BackendUrl).GetJson("/singleplayer/settings/defaultRaidSettings/");

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("SPTarkov.SinglePlayer: Received NULL response for DefaultRaidSettings. Defaulting to fallback.");
                return null;
            }

            Debug.LogError("SPTarkov.SinglePlayer: Successfully received DefaultRaidSettings");

            try
            {
                return JsonConvert.DeserializeObject<DefaultRaidSettings>(json);
            }
            catch (Exception exception)
            {
                Debug.LogError("SPTarkov.SinglePlayer: Failed to deserialize DefaultRaidSettings from server. Check your gameplay.json config in your server. Defaulting to fallback. Exception: " + exception);
                return null;
            }
        }
    }
}