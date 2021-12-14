using Aki.Common.Utils;
using Aki.SinglePlayer.Patches;
using Aki.Singleplayer.Patches.Dev;
using Aki.SinglePlayer.Utils.Bundles;

namespace Aki.SinglePlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Info("Loading: Aki.SinglePlayer");
            BundleSettings.GetBundles();
            PatchManager.Patches.EnableAll();
            PatchManager.Patches.Disable<CoordinatesPatch>();
        }
    }
}
