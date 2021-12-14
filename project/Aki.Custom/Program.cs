using Aki.Common.Utils;

namespace Aki.Custom
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Info("Loading: Aki.Custom");
            PatchManager.Patches.EnableAll();
        }
    }
}
