using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Diz.DependencyManager;
using UnityEngine;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Models;
using Aki.SinglePlayer.Utils.Bundles;
using IEasyBundle = GInterface263;
using IBundleLock = GInterface264;
using BindableState = GClass2251<Diz.DependencyManager.ELoadState>;

namespace Aki.SinglePlayer.Patches.Bundles
{
	public class EasyBundlePatch : GenericPatch<EasyBundlePatch>
	{
        static EasyBundlePatch()
        {
            _ = nameof(IEasyBundle.SameNameAsset);
            _ = nameof(IBundleLock.IsLocked);
            _ = nameof(BindableState.Bind);
        }

        public EasyBundlePatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
		{
            return Constants.EftTypes.Single(IsTargetType).GetConstructors()[0];
        }

        private static bool IsTargetType(Type type)
        {
            return type.IsClass && type.GetProperty("SameNameAsset") != null;
        }

        private static bool PatchPrefix(IEasyBundle __instance, string key, string rootPath, AssetBundleManifest manifest, IBundleLock bundleLock)
		{
            var easyBundle = new EasyBundleHelper(__instance);
            easyBundle.Key = key;

            var path = rootPath + key;
            var bundle = (BundleInfo)null;

            if (BundleSettings.Bundles.TryGetValue(key, out bundle))
            {
                path = bundle.Path;
            }

            easyBundle.Path = path;
            easyBundle.KeyWithoutExtension = Path.GetFileNameWithoutExtension(key);

            var dependencyKeys = manifest.GetDirectDependencies(key);

            foreach (KeyValuePair<string, BundleInfo> kvp in BundleSettings.Bundles)
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
