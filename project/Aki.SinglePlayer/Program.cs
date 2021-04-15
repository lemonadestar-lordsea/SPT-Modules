using UnityEngine;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Patches.Bots;
using Aki.SinglePlayer.Patches.Matchmaker;
using Aki.SinglePlayer.Patches.Progression;
using Aki.SinglePlayer.Patches.Quests;
using Aki.SinglePlayer.Patches.RaidFix;
using Aki.SinglePlayer.Patches.ScavMode;
//using Aki.Singleplayer.Patches.Debug;
using Aki.SinglePlayer.Patches.Bundles;
using Aki.SinglePlayer.Utils.Bundles;

namespace Aki.SinglePlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.LogError("Aki.SinglePlayer: Loaded");

            new BundleSettings();

            PatcherUtil.Patch<EasyAssetsPatch>();
            PatcherUtil.Patch<EasyBundlePatch>();
            PatcherUtil.Patch<BundleLoadPatch>();
            
			PatcherUtil.Patch<OfflineSaveProfilePatch>();
            PatcherUtil.Patch<OfflineSpawnPointPatch>();
            PatcherUtil.Patch<ExperienceGainPatch>();
            PatcherUtil.Patch<OnLoadRaidPatch>();
            PatcherUtil.Patch<WeaponDurabilityPatch>();
            PatcherUtil.Patch<SingleModeJamPatch>();
            
            PatcherUtil.Patch<Patches.Healing.MainMenuControllerPatch>();
			PatcherUtil.Patch<Patches.Healing.PlayerPatch>();

			PatcherUtil.Patch<MatchmakerOfflineRaidPatch>();
			PatcherUtil.Patch<MatchMakerSelectionLocationScreenPatch>();
			PatcherUtil.Patch<InsuranceScreenPatch>();

            PatcherUtil.Patch<BossSpawnChancePatch>();
			PatcherUtil.Patch<BotTemplateLimitPatch>();
            PatcherUtil.Patch<GetNewBotTemplatesPatch>();
            PatcherUtil.Patch<RemoveUsedBotProfilePatch>();
            PatcherUtil.Patch<SpawnPmcPatch>();
			PatcherUtil.Patch<CoreDifficultyPatch>();
			PatcherUtil.Patch<BotDifficultyPatch>();

            PatcherUtil.Patch<OnDeadPatch>();
            PatcherUtil.Patch<OnShellEjectEventPatch>();
            PatcherUtil.Patch<BotStationaryWeaponPatch>();

            PatcherUtil.Patch<BeaconPatch>();
			PatcherUtil.Patch<DogtagPatch>();

            PatcherUtil.Patch<LoadOfflineRaidScreenPatch>();
            PatcherUtil.Patch<ScavPrefabLoadPatch>();
            PatcherUtil.Patch<ScavProfileLoadPatch>();
            PatcherUtil.Patch<ScavExfilPatch>();

            PatcherUtil.Patch<EndByTimerPatch>();

            //PatcherUtil.Patch<CoordinatesPatch>();
        }
    }
}
