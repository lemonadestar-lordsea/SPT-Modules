using Aki.Bundles.Models;
using Aki.Bundles.Utils;
using Aki.Reflection.Patching;
using Diz.DependencyManager;
using UnityEngine.Build.Pipeline;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Aki.Bundles.Patches
{
    public class EasyBundlePatch : ModulePatch
    {
        static EasyBundlePatch()
        {
            _ = nameof(IEasyBundle.SameNameAsset);
            _ = nameof(IBundleLock.IsLocked);
            _ = nameof(BindableState<ELoadState>.Bind);
        }

        protected override MethodBase GetTargetMethod()
        {
            return EasyBundleHelper.Type.GetConstructors()[0];
        }

        [PatchPrefix]
        private static bool PatchPrefix(object __instance, string key, string rootPath, CompatibilityAssetBundleManifest manifest, IBundleLock bundleLock)
        {
            var path = rootPath + key;
            var dependencyKeys = manifest.GetDirectDependencies(key);

            if (BundleSettings.Bundles.TryGetValue(key, out BundleInfo bundle))
            {
                path = bundle.Path;
            }

            foreach (KeyValuePair<string, BundleInfo> kvp in BundleSettings.Bundles)
            {
                if (!key.Equals(kvp.Key))
                {
                    continue;
                }

                var result = dependencyKeys == null ? new List<string>() : dependencyKeys.ToList();
                dependencyKeys = result.Union(kvp.Value.DependencyKeys).ToArray();
                break;
            }

            _ = new EasyBundleHelper(__instance)
            {
                Key = key,
                Path = path,
                KeyWithoutExtension = Path.GetFileNameWithoutExtension(key),
                DependencyKeys = dependencyKeys,
                LoadState = new BindableState<ELoadState>(ELoadState.Unloaded, null),
                BundleLock = bundleLock
            };

            return false;
        }
    }
}
