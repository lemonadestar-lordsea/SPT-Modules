using Aki.Common.Utils;

namespace Aki.SinglePlayer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Info("Loading: Aki.SinglePlayer");
            PatchManager.Patches.EnableAll();
        }
    }
}
