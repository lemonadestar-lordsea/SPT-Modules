using UnityEngine;
using Aki.SinglePlayer.Utils.Bundles;
using Aki.SinglePlayer.Patches.Bots;
using Aki.SinglePlayer.Patches.Bundles;
using Aki.SinglePlayer.Patches.Healing;
using Aki.SinglePlayer.Patches.Matchmaker;
using Aki.SinglePlayer.Patches.Progression;
using Aki.SinglePlayer.Patches.Quests;
using Aki.SinglePlayer.Patches.ScavMode;

namespace Aki.SinglePlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.LogError("Aki.SinglePlayer: Loaded");

            BundleSettings.GetBundles();

            new EasyAssetsPatch().Apply();
            new EasyBundlePatch().Apply();
            new BundleLoadPatch().Apply();
            new OfflineSaveProfilePatch().Apply();
            new OfflineSpawnPointPatch().Apply();
            new ExperienceGainPatch().Apply();
            new OnLoadRaidPatch().Apply();
            new MainMenuControllerPatch().Apply();
            new PlayerPatch().Apply();
            new MatchmakerOfflineRaidPatch().Apply();
            new MatchMakerSelectionLocationScreenPatch().Apply();
            new InsuranceScreenPatch().Apply();
            new BotTemplateLimitPatch().Apply();
            new GetNewBotTemplatesPatch().Apply();
            new RemoveUsedBotProfilePatch().Apply();
            new SpawnPmcPatch().Apply();
            new CoreDifficultyPatch().Apply();
            new BotDifficultyPatch().Apply();
            new BossSpawnChancePatch().Apply();
            new BeaconPatch().Apply();
            new DogtagPatch().Apply();
            new LoadOfflineRaidScreenPatch().Apply();
            new ScavPrefabLoadPatch().Apply();
            new ScavProfileLoadPatch().Apply();
            new ScavExfilPatch().Apply();
            new EndByTimerPatch().Apply();
        }
    }
}
