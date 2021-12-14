using Aki.Reflection.Patching;
using Aki.SinglePlayer.Patches.Bots;
using Aki.SinglePlayer.Patches.Bundles;
using Aki.Singleplayer.Patches.Dev;
using Aki.SinglePlayer.Patches.Healing;
using Aki.SinglePlayer.Patches.Https;
using Aki.SinglePlayer.Patches.MainMenu;
using Aki.SinglePlayer.Patches.Progression;
using Aki.SinglePlayer.Patches.Quests;
using Aki.SinglePlayer.Patches.RaidFix;
using Aki.SinglePlayer.Patches.ScavMode;

namespace Aki.SinglePlayer
{
    public static class PatchManager
    {
        public static readonly PatchList Patches;

        static PatchManager()
        {
            Patches = new PatchList
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
                new InsuranceScreenPatch(),
                new BotTemplateLimitPatch(),
                new GetNewBotTemplatesPatch(),
                new RemoveUsedBotProfilePatch(),
                new SpawnPmcPatch(),
                new CoreDifficultyPatch(),
                new BotDifficultyPatch(),
                new BossSpawnChancePatch(),
                new DogtagPatch(),
                new LoadOfflineRaidScreenPatch(),
                new ScavPrefabLoadPatch(),
                new ScavProfileLoadPatch(),
                new ScavExfilPatch(),
                new ExfilPointManagerPatch(),
                new EndByTimerPatch(),
                new CoordinatesPatch(),
                new VersionLabelPatch(),
                new WebSocketPatch(),
                new TinnitusFixPatch()
            };
        }
    }
}
