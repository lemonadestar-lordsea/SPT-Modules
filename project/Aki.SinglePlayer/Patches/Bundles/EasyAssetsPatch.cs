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
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var type = typeof(EasyAssets);

            _manifestField = type.GetField(nameof(EasyAssets.Manifest));
            _bundlesField = type.GetField($"{EasyBundleHelper.Type.Name.ToLower()}_0", flags);
            _systemProperty = type.GetProperty("System");

            return typeof(EasyAssets).GetMethods(Constants.PrivateFlags).Single(IsTargetMethod);
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length != 5
                || parameters[0].Name != "bundleLock"
                || parameters[1].Name != "defaultKey"
                || parameters[4].Name != "shouldExclude") ? false : true;
        }

        private static bool PatchPrefix(ref Task __result, EasyAssets __instance, [CanBeNull] IBundleLock bundleLock, string defaultKey, string rootPath,
            string platformName, [CanBeNull] Func<string, bool> shouldExclude)
        {
            __result = Init(__instance, bundleLock, defaultKey, rootPath, platformName, shouldExclude);
            return false;
        }

        private static async Task Init(EasyAssets instance, [CanBeNull] IBundleLock bundleLock, string defaultKey, string rootPath,
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
            var bundleNames = manifest.GetAllAssetBundles().Union(resourcesModbundles).ToArray();
            var bundles = (IEasyBundle[])Array.CreateInstance(EasyBundleHelper.Type, bundleNames.Length);

            if (bundleLock == null)
            {
                bundleLock = new BundleLock(int.MaxValue);
            }

            for (var i = 0; i < bundleNames.Length; i++)
            {
                bundles[i] = (IEasyBundle)Activator.CreateInstance(EasyBundleHelper.Type, new object[] { bundleNames[i], path, manifest, bundleLock });
                await JobScheduler.Yield();
            }

            _manifestField.SetValue(instance, manifest);
            _bundlesField.SetValue(instance, bundles);
            _systemProperty.SetValue(instance, new DependencyGraph(bundles, defaultKey, shouldExclude));
        }
    }
}
