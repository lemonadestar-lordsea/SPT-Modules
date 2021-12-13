using Aki.Reflection.Patching;

namespace Aki.Core.Patches
{
    public class PatchManager
    {
        private readonly PatchList _patches;

        public PatchManager()
        {
            _patches = new PatchList
            {
                new ConsistencySinglePatch(),
                new ConsistencyMultiPatch(),
                new BattlEyePatch(),
                new SslCertificatePatch(),
                new UnityWebRequestPatch()
            };
        }

        public void RunPatches()
        {
            _patches.EnableAll();
        }
    }
}
