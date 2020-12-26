/* ModuleLoader.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Aki.Loader
{
    static class ModuleLoader
    {
        private static List<string> repositories;

        static ModuleLoader()
        {
            repositories = new List<string>();
        }

        static void ExecuteAssembly(string filepath)
        {
            try
            {
                var asm = Assembly.Load(File.ReadAllBytes(filepath));
                var t = asm.GetTypes().Single(ti => ti.Name == "Program");
                var o = asm.CreateInstance(t.Name);
                var mi = t.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic);

                mi.Invoke(o, new object[] { new string[] { filepath } });

                Debug.LogError($"Aki.Loader: Loaded {filepath}");
            }
            catch
            {
                Debug.LogError($"Aki.Loader: Cannot load {filepath}");
            }
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
                        ExecuteAssembly(file);
                    }
                }
            }
        }
    }
}
