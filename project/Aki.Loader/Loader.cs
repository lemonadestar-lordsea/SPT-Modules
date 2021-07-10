using System;
using System.Collections.Generic;
using Aki.Common.Utils;

namespace Aki.Loader
{
    public static class Loader
    {
        private static readonly List<string> _repositories;

        static Loader()
        {
            _repositories = new List<string>();
        }

        public static void AddRepository(string path)
        {
            if (VFS.Exists(path) && VFS.GetDirectories(path).Length > 0)
            {
                _repositories.Add(path);
            }
        }

        public static void LoadAllAssemblies()
        {
            foreach (var repository in _repositories)
            {
                var dirs = VFS.GetDirectories(repository);

                foreach (var dir in dirs)
                {
                    var file = VFS.Combine(dir, "./module.dll");

                    if (VFS.Exists(file))
                    {
                        try
                        {
                            RunUtil.LoadAndRun(file);
                            Log.Info($"Aki.Loader: Successfully loaded '{file}'");
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Aki.Loader: Failed to load '{file}'");
                            Log.Write(ex.Message);
                            Log.Write(ex.StackTrace);
                        }
                    }
                    else
                    {
                        Log.Error($"Aki.Loader: Failed to find module.dll in '{dir}'");
                    }
                }
            }
        }
    }
}
