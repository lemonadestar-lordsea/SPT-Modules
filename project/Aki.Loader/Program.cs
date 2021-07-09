using Aki.Common.Utils;
using System;

namespace Aki.Loader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Info("Executing Aki.Loader.Program.Main()");

            try
            {
                Log.Info("Aki.Loader: Loaded");

                ModuleLoader.AddRepository(VFS.Combine(VFS.Cwd, "Aki_Data/Modules/"));
                ModuleLoader.AddRepository(VFS.Combine(VFS.Cwd, "user/mods/"));
                ModuleLoader.LoadAllAssemblies();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.GetType().Name}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            Log.Info("Done with Aki.Loader.Program.Main()");
        }
    }
}
