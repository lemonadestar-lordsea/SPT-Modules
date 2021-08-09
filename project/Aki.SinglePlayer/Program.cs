using Aki.Common;
using Aki.SinglePlayer.Patches;
using Aki.SinglePlayer.Utils.Bundles;

namespace Aki.SinglePlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Info("Loading: Aki.SinglePlayer");

            BundleSettings.GetBundles();

            var patcher = new PatchManager();
            patcher.RunPatches();
        }
    }
}
