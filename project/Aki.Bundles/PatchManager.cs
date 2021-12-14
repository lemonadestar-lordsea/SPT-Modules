using Aki.Bundles.Patches;
using Aki.Reflection.Patching;

namespace Aki.Bundles
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
                new BundleLoadPatch()
            };
        }
    }
}
