using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using JetBrains.Annotations;
using Diz.Jobs;
using Diz.Resources;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Models;
using Aki.SinglePlayer.Utils.Bundles;
using IEasyBundle = GInterface263;
using IBundleLock = GInterface264;
using BundleLock = GClass2265;
using DependencyGraph = GClass2266<GInterface263>;

namespace Aki.SinglePlayer.Patches.Bundles
{
    public class EasyAssetsPatch : GenericPatch<EasyAssetsPatch>
    {
        private static Type _easyBundleType;
        private static FieldInfo _manifestField;
        private static FieldInfo _bundlesField;
        private static PropertyInfo _systemProperty;

        public EasyAssetsPatch() : base(prefix: nameof(PatchPrefix))
        {
            _ = nameof(IEasyBundle.SameNameAsset);
            _ = nameof(IBundleLock.IsLocked);
            _ = nameof(BundleLock.MaxConcurrentOperations);
            _ = nameof(DependencyGraph.GetDefaultNode);
        }

        protected override MethodBase GetTargetMethod()
        {
            _easyBundleType = Constants.EftTypes.Single(type => type.IsClass && type.GetProperty("SameNameAsset") != null);
            _manifestField = typeof(EasyAssets).GetField(nameof(EasyAssets.Manifest));
            _bundlesField = typeof(EasyAssets).GetField($"{_easyBundleType.Name.ToLower()}_0");
            _systemProperty = typeof(EasyAssets).GetProperty("System");

            return typeof(EasyAssets).GetMethods().Single(IsTargetMethod);
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length == 5
                && parameters[0].Name == "bundleLock" 
                && parameters[1].Name == "defaultKey"
                && parameters[4].Name == "shouldExclude");
        }

        private static bool PatchPrefix(EasyAssets __instance, [CanBeNull] IBundleLock bundleLock, string defaultKey, string rootPath,
                                        string platformName, [CanBeNull] Func<string, bool> shouldExclude, ref Task __result)
        {
            __result = Init(__instance, bundleLock, defaultKey, rootPath, platformName, shouldExclude);
            return false;
        }

        private static async Task Init(EasyAssets __instance, [CanBeNull] IBundleLock bundleLock, string defaultKey, string rootPath,
                                      string platformName, [CanBeNull] Func<string, bool> shouldExclude)
        {
            var path = $"{rootPath.Replace("file:///", string.Empty).Replace("file://", string.Empty)}/{platformName}/";
            var manifestLoading = AssetBundle.LoadFromFileAsync(path + platformName);
            await manifestLoading.Await();

            var assetBundle = manifestLoading.assetBundle;
            var assetLoading = assetBundle.LoadAllAssetsAsync();
            await assetLoading.Await();

            // add ModManifest
            var resourcesModbundles = new List<string>();

            foreach (KeyValuePair<string, BundleInfo> kvp in BundleSettings.Bundles)
            {
                resourcesModbundles.Add(kvp.Key);
            }

            var manifest = (AssetBundleManifest)assetLoading.allAssets[0];
            var bundleNames = manifest.GetAllAssetBundles().ToList().Union(resourcesModbundles).ToList().ToArray<string>();
            var bundles = (IEasyBundle[])Array.CreateInstance(_easyBundleType, bundleNames.Length);

            if (bundleLock == null)
            {
                bundleLock = new BundleLock(int.MaxValue);
            }

            for (var i = 0; i < bundleNames.Length; i++)
            {
                bundles[i] = (IEasyBundle)Activator.CreateInstance(_easyBundleType, new object[] { bundleNames[i], path, manifest, bundleLock });
                await JobScheduler.Yield();
            }

            _manifestField.SetValue(__instance, manifest);
            _bundlesField.SetValue(__instance, bundles);
            _systemProperty.SetValue(__instance, new DependencyGraph(bundles, defaultKey, shouldExclude));
        }
    }
}
