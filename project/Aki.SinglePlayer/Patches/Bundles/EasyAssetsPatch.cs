using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using JetBrains.Annotations;
using Diz.Jobs;
using Diz.Resources;
using Aki.Common.Utils.Patching;
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
        private static string _bundlesFieldName;

        public EasyAssetsPatch() : base(prefix: nameof(PatchPrefix))
        {
            _ = nameof(IEasyBundle.SameNameAsset);
            _ = nameof(IBundleLock.IsLocked);
            _ = nameof(BundleLock.MaxConcurrentOperations);
            _ = nameof(DependencyGraph.GetDefaultNode);
        }

        protected override MethodBase GetTargetMethod()
        {
            _easyBundleType = PatcherConstants.TargetAssembly.GetTypes().Single(type => type.IsClass && type.GetProperty("SameNameAsset") != null);
            _bundlesFieldName = $"{_easyBundleType.Name.ToLower()}_0";

            var targetType = PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType);
            return AccessTools.GetDeclaredMethods(targetType).Single(IsTargetMethod);
        }

        private static bool IsTargetType(Type type)
        {
            var fields = type.GetFields();

            if (fields.Length > 2)
            {
                return false;
            }

            if (!fields.Any(x => x.Name == "Manifest"))
            {
                return false;
            }

            return type.GetMethod("Create", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly) != null;
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length != 5 || parameters[0].Name != "bundleLock" || parameters[1].Name != "defaultKey" || parameters[4].Name != "shouldExclude") ? false : true;
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
            var traverse = Traverse.Create(__instance);
            var path = $"{rootPath.Replace("file:///", "").Replace("file://", "")}/{platformName}/";
            
            var manifestLoading = AssetBundle.LoadFromFileAsync(path + platformName);
            await manifestLoading.Await();

            var assetBundle = manifestLoading.assetBundle;
            var assetLoading = assetBundle.LoadAllAssetsAsync();
            await assetLoading.Await();

            traverse.Field<AssetBundleManifest>("Manifest").Value = (AssetBundleManifest)assetLoading.allAssets[0];
            var manifest = traverse.Field<AssetBundleManifest>("Manifest").Value;

            // add ModManifest
            var result = manifest.GetAllAssetBundles().ToList<string>();
            var resourcesModbundles = new List<string>();

            foreach (KeyValuePair<string, BundleInfo> kvp in BundleSettings.Bundles)
            {
                resourcesModbundles.Add(kvp.Key);
            }

            var bundleNames = result.Union(resourcesModbundles).ToList<string>().ToArray<string>();

            traverse.Field(_bundlesFieldName).SetValue(Array.CreateInstance(_easyBundleType, bundleNames.Length));

            if (bundleLock == null)
            {
                bundleLock = new BundleLock(int.MaxValue);
            }

            var bundles = traverse.Field(_bundlesFieldName).GetValue<IEasyBundle[]>();

            for (var i = 0; i < bundleNames.Length; i++)
            {
                bundles[i] = (IEasyBundle)Activator.CreateInstance(_easyBundleType, new object[] { bundleNames[i], path, manifest, bundleLock });
                await JobScheduler.Yield();
            }

            traverse.Field(_bundlesFieldName).SetValue(bundles);
            traverse.Property<DependencyGraph>("System").Value = new DependencyGraph(bundles, defaultKey, shouldExclude);
        }
    }
}
