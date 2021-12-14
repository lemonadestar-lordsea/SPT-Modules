using Aki.Bundles.Utils;
using Aki.Common.Utils;

namespace Aki.Bundles
{
    public class Program
    {
        static void Main(string[] args)
        {
            Log.Info("Loading: Aki.Bundles");
            BundleSettings.GetBundles();
            PatchManager.Patches.EnableAll();
        }
    }
}
