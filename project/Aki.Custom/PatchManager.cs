using Aki.Custom.Patches;
using Aki.Reflection.Patching;

namespace Aki.Custom
{
    public static class PatchManager
    {
        public static readonly PatchList Patches;

        static PatchManager()
        {
            Patches = new PatchList
            {
                new BossSpawnChancePatch(),
                new BotDifficultyPatch(),
                new CoreDifficultyPatch(),
                new OfflineRaidMenuPatch(),
                new SessionIdPatch(),
                new VersionLabelPatch(),
            };
        }
    }
}
