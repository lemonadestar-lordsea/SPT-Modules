using Aki.Core.Patches;
using Aki.Reflection.Patching;

namespace Aki.Core
{
    public static class PatchManager
    {
        public static readonly PatchList Patches;

        static PatchManager()
        {
            Patches = new PatchList
            {
                new ConsistencySinglePatch(),
                new ConsistencyMultiPatch(),
                new BattlEyePatch(),
                new SslCertificatePatch(),
                new UnityWebRequestPatch()
            };
        }
    }
}
