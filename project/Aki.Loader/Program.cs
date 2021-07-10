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

                Loader.AddRepository(VFS.Combine(VFS.Cwd, "Aki_Data/Modules/"));
                Loader.AddRepository(VFS.Combine(VFS.Cwd, "user/mods/"));
                Loader.LoadAllAssemblies();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.GetType().Name}: {ex.Message}{Environment.NewLine}{ex.StackTrace}");
            }

            Log.Info("Done with Aki.Loader.Program.Main()");
        }
    }
}
