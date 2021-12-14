﻿using Aki.Reflection.Patching;
using Aki.SinglePlayer.Patches.Healing;
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
                new OfflineSaveProfilePatch(),
                new OfflineSpawnPointPatch(),
                new ExperienceGainPatch(),
                new MainMenuControllerPatch(),
                new PlayerPatch(),
                new SelectLocationScreenPatch(),
                new InsuranceScreenPatch(),
                new BotTemplateLimitPatch(),
                new GetNewBotTemplatesPatch(),
                new RemoveUsedBotProfilePatch(),
                new DogtagPatch(),
                new LoadOfflineRaidScreenPatch(),
                new ScavPrefabLoadPatch(),
                new ScavProfileLoadPatch(),
                new ScavExfilPatch(),
                new ExfilPointManagerPatch(),
                new TinnitusFixPatch()
            };
        }
    }
}
