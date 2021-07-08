using System;
using System.Collections.Generic;
using UnityEngine;
using Aki.Common.Utils;

namespace Aki.Loader
{
    static class ModuleLoader
    {
        private static readonly List<string> _repositories;

        static ModuleLoader()
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
                    var file = $"{dir}/module.dll";

                    if (VFS.Exists(file))
                    {
                        try
                        {
                            RunUtil.LoadAndRun(file);
                            Debug.LogError($"Aki.Loader: Loaded '{file}'!");
                        }
                        catch(Exception ex)
                        {
                            Debug.LogError($"Aki.Loader: Failed to load '{file}'! Exception below.");
                            Debug.LogError(ex);
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
