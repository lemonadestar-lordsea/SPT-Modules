using System;
using System.Collections.Generic;
using Aki.Common.Utils;

namespace Aki.Loader
{
    public static class Loader
    {
        private static List<string> _repositories;

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

        public static void LoadAssembly(string filepath)
        {
            try
            {
                RunUtil.LoadAndRun(filepath);
                Program.Logger.LogInfo($"Aki.Loader: Successfully loaded '{filepath}'");
            }
            catch (Exception ex)
            {
                Program.Logger.LogError($"Aki.Loader: Failed to load '{filepath}'");
                Program.Logger.LogFatal(ex);
            }
        }

        public static void LoadRepository(string repository)
        {
            var files = new List<string>();
            var dirs = VFS.GetDirectories(repository);

            foreach (var dir in dirs)
            {
                var file = VFS.Combine(dir, "./module.dll");

                if (VFS.Exists(file))
                {
                    Program.Logger.LogInfo($"Aki.Loader: Found module.dll in '{dir}'");
                    files.Add(file);
                }
            }

            foreach (var filepath in files)
            {
                LoadAssembly(filepath);
            }
        }

        public static void LoadAllAssemblies()
        {
            foreach (var repository in _repositories)
            {
                LoadRepository(repository);
            }
        }
    }
}
