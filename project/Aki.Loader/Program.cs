using Aki.Common.Utils;
using BepInEx.Logging;

namespace Aki.Loader
{
    public class Program
    {
        public static ManualLogSource Logger { get; private set; }

        public static void Main(string[] args)
        {
            Logger = BepInEx.Logging.Logger.CreateLogSource("AKI.Loader");

            Logger.LogInfo("Loading: Aki.Loader");

            Loader.AddRepository(VFS.Combine(VFS.Cwd, "Aki_Data/Modules/"));
            Loader.AddRepository(VFS.Combine(VFS.Cwd, "user/mods/"));
            Loader.LoadAllAssemblies();
        }
    }
}
