using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Aki.Common.Utils;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils;
using Aki.SinglePlayer.Utils.Bundles;

namespace Aki.SinglePlayer.Patches.Bundles
{
    public class BundleLoadPatch : GenericPatch<BundleLoadPatch>
    {
        public BundleLoadPatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Single(IsTargetMethod);
        }

        // Locate the target class; must have a Property named "SameNameAsset"
        private static bool IsTargetType(Type type)
        {
            return type.IsClass && type.GetProperty("SameNameAsset") != null;
        }

        // Locate the target method; must be private, not have parameters and must return a Task
        private static bool IsTargetMethod(MethodInfo method)
        {
            return method.GetParameters().Length == 0 && method.ReturnType == typeof(Task);
        }

        // Execute this code instead of original
        static bool PatchPrefix(object __instance,string ___string_1, ref Task __result)
        {
            if (___string_1.IndexOf("http") == -1)
            {
                return true;
            }

            __result = LoadBundleFromServer(__instance);
            return false;
        }

        private static async Task LoadBundleFromServer(object __instance)
        {
            var easyBundle = new EasyBundleHelper(__instance);
            var path = easyBundle.Path;
            var bundleKey = Regex.Split(path, "bundle/", RegexOptions.IgnoreCase)[1];
            var cachePath = BundleSettings.cachePach;
            var filepath = cachePath + bundleKey;

            if (path.Contains("http"))
            {
                var data = RequestHandler.GetBundle(path);
                
                if (data != null)
                {
                    VFS.WriteFile(filepath, data);
                    easyBundle.Path = filepath;
                }
            }

            await easyBundle.LoadingCoroutine();
        }
    }
}
