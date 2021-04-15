using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Diz.DependencyManager;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils.Bundles;
using IEasyBundle = GInterface253;                                  // Property: SameNameAsset 
using IBundleLock = GInterface254;                                  // Property: IsLocked
using BindableState = GClass2206<Diz.DependencyManager.ELoadState>; // Construct method parameter: initialValue

namespace Aki.SinglePlayer.Patches.Bundles
{
	public class EasyBundlePatch : GenericPatch<EasyBundlePatch>
	{
        public EasyBundlePatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
		{
            return PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType).GetConstructors()[0];
        }

        private static bool IsTargetType(Type type)
        {
            return type.IsClass && type.GetProperty("SameNameAsset") != null;
        }

        static bool PatchPrefix(IEasyBundle __instance, string key, string rootPath, UnityEngine.AssetBundleManifest manifest, IBundleLock bundleLock)
		{
            var easyBundle = new EasyBundleHelper(__instance);
            easyBundle.Key = key;

            var path = rootPath + key;
            var bundle = (BundleInfo)null;

            if (BundleSettings.bundles.TryGetValue(key, out bundle))
            {
                path = bundle.Path;
            }

            easyBundle.Path = path;
            easyBundle.KeyWithoutExtension = Path.GetFileNameWithoutExtension(key);

            var dependencyKeys = manifest.GetDirectDependencies(key);

            foreach (KeyValuePair<string, BundleInfo> kvp in BundleSettings.bundles)
            {
                if (!key.Equals(kvp.Key))
                {
                    continue;
                }

                var result = dependencyKeys == null ? new List<string>() : dependencyKeys.ToList();
                dependencyKeys = result.Union(kvp.Value.DependencyKeys).ToList().ToArray<string>();
                break;
            }

            easyBundle.DependencyKeys = dependencyKeys;
            easyBundle.LoadState = new BindableState(ELoadState.Unloaded, null);
            easyBundle.BundleLock = bundleLock;

            return false;
		}
	}
}
