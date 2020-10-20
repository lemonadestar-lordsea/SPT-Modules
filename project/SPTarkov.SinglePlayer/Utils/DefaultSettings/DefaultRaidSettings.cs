/* DefaultRaidSettings.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using EFT.Bots;

namespace SPTarkov.SinglePlayer.Utils.DefaultSettings
{
    public class DefaultRaidSettings
    {
        public EBotAmount AiAmount;
        public EBotDifficulty AiDifficulty;
        public bool BossEnabled;
        public bool ScavWars;
        public bool TaggedAndCursed;

        public DefaultRaidSettings(EBotAmount aiAmount, EBotDifficulty aiDifficulty, bool bossEnabled, bool scavWars, bool taggedAndCursed)
        {
            AiAmount = aiAmount;
            AiDifficulty = aiDifficulty;
            BossEnabled = bossEnabled;
            ScavWars = scavWars;
            TaggedAndCursed = taggedAndCursed;
        }
    }
}
