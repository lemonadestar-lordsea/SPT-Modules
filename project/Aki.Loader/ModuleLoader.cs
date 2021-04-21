using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Aki.Loader
{
    static class ModuleLoader
    {
        private static readonly List<string> repositories;

        static ModuleLoader()
        {
            repositories = new List<string>();
        }

        public static void AddRepository(string path)
        {
            if (Directory.Exists(path) && Directory.GetDirectories(path).Length > 0)
            {
                repositories.Add(path);
            }
        }

        public static void LoadAllAssemblies()
        {
            foreach (var repository in repositories)
            {
                var dirs = Directory.GetDirectories(repository);

                foreach (var dir in dirs)
                {
                    var file = $"{dir}/module.dll";

                    if (File.Exists(file))
                    {
                        Exception error = RunUtil.LoadAndRun(file);
                        
                        if (error == null)
                        {
                            Debug.LogError($"Aki.Loader: Loaded '{file}'!");
                        }
                        else
                        {
                            Debug.LogError($"Aki.Loader: Failed to load '{file}'! Exception below.");
                            Debug.LogError(error);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Aki.Loader: Failed to find module.dll in '{dir}'");
                    }
                }
            }
        }
    }
}
