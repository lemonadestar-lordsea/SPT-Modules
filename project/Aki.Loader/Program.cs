using Aki.Common.Utils;

namespace Aki.Loader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Info("Aki.Loader: Loaded");

            ModuleLoader.AddRepository(VFS.Combine(VFS.Cwd, "/Aki_Data/Modules/"));
            ModuleLoader.AddRepository(VFS.Combine(VFS.Cwd, "/user/mods/"));
            ModuleLoader.LoadAllAssemblies();
        }
    }
}
