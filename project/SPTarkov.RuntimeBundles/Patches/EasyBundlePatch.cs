using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Diz.DependencyManager;
using SPTarkov.Common.Utils.Patching;
using SPTarkov.RuntimeBundles.Utils;
using IEasyBundle = GInterface223; //Property: SameNameAsset 
using IBundleLock = GInterface224; //Property: IsLocked
using BindableState = GClass2084<Diz.DependencyManager.ELoadState>; //Construct method parameter: initialValue

namespace SPTarkov.RuntimeBundles.Patches
{
	public class EasyBundlePatch : GenericPatch<EasyBundlePatch>
	{
        public EasyBundlePatch() : base(prefix: nameof(PatchPrefix)) {}

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
            EasyBundleHelper esayBundle = new EasyBundleHelper(__instance);
            esayBundle.Key = key;

            var path = rootPath + key;
            BundleInfo bundle;
            if(Settings.bundles.TryGetValue(key, out bundle))
            {
                path = bundle.Path;
            }

            esayBundle.Path = path;
            esayBundle.KeyWithoutExtension = Path.GetFileNameWithoutExtension(key);

            string[] dependencyKeys = manifest.GetDirectDependencies(key);


            foreach (KeyValuePair<string, BundleInfo> kvp in Settings.bundles)
            {
                if (!key.Equals(kvp.Key))
                    continue;

                List<string> result = dependencyKeys == null ? new List<string>() : dependencyKeys.ToList<string>();
                dependencyKeys = result.Union(kvp.Value.DependencyKeys).ToList<string>().ToArray<string>();
                break;
            }

            esayBundle.DependencyKeys = dependencyKeys;
            esayBundle.LoadState = new BindableState(ELoadState.Unloaded, null);
            esayBundle.BundleLock = bundleLock;

            return false;
		}
	}
}
