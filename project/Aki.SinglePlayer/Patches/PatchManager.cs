﻿using Aki.Reflection.Patching;
using Aki.SinglePlayer.Patches.Bots;
using Aki.SinglePlayer.Patches.Healing;
using Aki.SinglePlayer.Patches.MainMenu;
using Aki.SinglePlayer.Patches.Progression;
using Aki.SinglePlayer.Patches.Quests;
using Aki.SinglePlayer.Patches.ScavMode;
using Aki.SinglePlayer.Patches.Bundles;

namespace Aki.SinglePlayer.Patches
{
    public class PatchManager
    {
        private readonly PatchList _patches;

        public PatchManager()
        {
            _patches = new PatchList
            {
                new EasyAssetsPatch(),
                new EasyBundlePatch(),
                new BundleLoadPatch(),
                new OfflineSaveProfilePatch(),
                new OfflineSpawnPointPatch(),
                new ExperienceGainPatch(),
                new OnLoadRaidPatch(),
                new MainMenuControllerPatch(),
                new PlayerPatch(),
                new MatchmakerOfflineRaidPatch(),
                new SelectLocationScreenPatch(),
                //new FieldOfViewPatch(),
                new InsuranceScreenPatch(),
                new BotTemplateLimitPatch(),
                new GetNewBotTemplatesPatch(),
                new RemoveUsedBotProfilePatch(),
                new SpawnPmcPatch(),
                new CoreDifficultyPatch(),
                new BotDifficultyPatch(),
                new BossSpawnChancePatch(),
                //new BeaconPatch(),
                new DogtagPatch(),
                new LoadOfflineRaidScreenPatch(),
                new ScavPrefabLoadPatch(),
                new ScavProfileLoadPatch(),
                new ScavExfilPatch(),
                new ExfilPointManagerPatch(),
                new EndByTimerPatch(),
                //new CoordinatesPatch(),
                new VersionLabelPatch(),
            };
        }

        public void RunPatches()
        {
            _patches.EnableAll();
        }
    }
}
