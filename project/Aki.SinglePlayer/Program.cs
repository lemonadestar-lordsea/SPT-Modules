using Aki.Common.Utils;
using Aki.Singleplayer.Patches.Dev;

namespace Aki.SinglePlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Info("Loading: Aki.SinglePlayer");
            PatchManager.Patches.EnableAll();
            PatchManager.Patches.Disable<CoordinatesPatch>();
        }
    }
}
