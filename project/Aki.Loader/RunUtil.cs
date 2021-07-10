/*
 * Loads and runs an assembly file.
 * Assembly file is expected to have a static function called
 * Main() with either no args or a string[] array.
 * For example: public static void Main(string[] args) is valid.
 * Also: private static void Main() is also valid.
 *
 * Loads all required dependencies as well.
 *
 * Author Epicguru.
 */

using System;
using System.IO;
using System.Reflection;
using Aki.Common.Utils;

namespace Aki.Loader
{
    public static class RunUtil
    {
        private static string _depDir;
        private static bool _hasHooked = false;

        public static void LoadAndRun(string dllPath, params string[] args)
        {
            if (!_hasHooked)
            {
                AppDomain.CurrentDomain.AssemblyResolve += DomainAssemblyResolve;
                _hasHooked = true;
            }

            MethodInfo entry = null;
            bool hasStringArray = false;

            LoadAssemblyAndEntryPoint(dllPath, out entry, out hasStringArray);

            try
            {
                entry.Invoke(null, hasStringArray ? new object[] { args } : new object[0]);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);

                if (ex.InnerException != null)
                {
                    Log.Error(ex.InnerException);
                }
            }
        }

        private static void LoadAssemblyAndEntryPoint(string dllPath, out MethodInfo entryPoint, out bool hasStringArray)
        {
            Assembly asm;
            entryPoint = null;
            hasStringArray = false;

            try
            {
                asm = LoadAssembly(dllPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load assembly {dllPath}", ex);
            }

            try
            {
                entryPoint = FindMainFunction(asm, out hasStringArray);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to find entry point in {asm.FullName}", ex);
            }

            try
            {
                LoadDependencies(asm, new FileInfo(dllPath).DirectoryName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load dependencies in {asm.FullName}", ex);
            }
        }

        private static Assembly LoadAssembly(string dllPath)
        {
            byte[] bytes;
            Assembly asm;

            try
            {
                bytes = File.ReadAllBytes(dllPath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to read assembly bytes from '{dllPath}'", ex);
            }

            try
            {
                asm = Assembly.Load(bytes);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed creating assembly from file. Duplicate assembly?", ex);
            }

            return asm;
        }

        private static bool IsLoaded(AssemblyName name)
        {
            var domain = AppDomain.CurrentDomain;

            foreach (var item in domain.GetAssemblies())
            {
                // TODO make this comparison better.
                if (item.ToString() == name.ToString())
                {
                    return true;
                }
            }

            return false;
        }

        private static void LoadDependencies(Assembly a, string sourceFolder)
        {
            var domain = AppDomain.CurrentDomain;
            var references = a.GetReferencedAssemblies();

            foreach (var item in references)
            {
                if (!IsLoaded(item))
                {
                    Assembly created;

                    try
                    {
                        _depDir = sourceFolder;
                        created = domain.Load(item);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Failed to load dependency {item.FullName}", ex);
                    }

                    // Note: source folder never changes, so all deps are expected be be in the same folder as main dll.
                    // For example, if A depends on B and B depends on C then
                    // when loading A, B.dll and C.dll should be in the same folder as A.dll
                    LoadDependencies(created, sourceFolder);
                }
            }
        }

        private static MethodInfo FindMainFunction(Assembly a, out bool hasStringArray)
        {
            foreach (var type in a.GetTypes())
            {
                if (!type.IsClass)
                {
                    continue;
                }

                if (type.IsGenericType)
                {
                    continue;
                }

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (method.IsGenericMethod)
                    {
                        continue;
                    }

                    // Must be called Main just like regular program.
                    if (method.Name == "Main")
                    {
                        // Allowed parameters: none, or an array of strings (such as string[] args)
                        var args = method.GetParameters();

                        if (args.Length == 0)
                        {
                            hasStringArray = false;
                            return method;
                        }

                        if (args.Length == 1 && args[0].ParameterType == typeof(string[]))
                        {
                            hasStringArray = true;
                            return method;
                        }
                    }
                }
            }

            hasStringArray = false;
            return null;
        }

        private static Assembly DomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string root = _depDir;
            string dllName = $"{args.Name.Split(',')[0].Trim()}.dll";
            string path = Path.Combine(root, dllName);

            Log.Info($"Loading dependency '{dllName}' from '{root}'... ");
            return LoadAssembly(path);
        }
    }
}