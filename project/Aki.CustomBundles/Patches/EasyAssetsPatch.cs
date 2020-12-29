/* EasyAssetsPatch.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Craink
 * reider123
 * GGaulin
 * Ginja
 * Merijn Hendriks
 */


using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using Diz.Jobs;
using Diz.Resources;
using JetBrains.Annotations;
using Aki.Common.Utils.Patching;
using Aki.CustomBundles.Utils;
using IEasyBundle = GInterface249; //Property: SameNameAsset 
using IBundleLock = GInterface250; //Property: IsLocked
using BundleLock = GClass2174; //Property: MaxConcurrentOperations
using DependencyGraph = GClass2175<GInterface249>; // Method: GetDefaultNode()

/* Maintenance Tips
 * 
 * This patch is used to change behaivior of the "Diz Plugings - Achievements System"
 * The target class is called "EasyAssets", this patch will replace portions of the existing class and will not run the original code after.
 * 
 * Use dnSpy to find the correct GClass/GInterface/Property Name used within each patch.
 * 
 * dnSpy:
 *   - Open the un-obfuscated EFT CSharp Assemply "\EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll"
 *   - Within the Assembly Expoler Tress, select the "Assembly-CSharp (0.0.0.0) file
 *   - Search for "SameNameAsset"           using Options Search For: "Property", "Selected Files" and update "IEasyBundle" to the Interface found
 *   - Search for "IsLocked"                using Options Search For: "Property", "Selected Files" and update "IBundleLock" to the Interface found
 *   - Search for "MaxConcurrentOperations" using Options Search For: "Property", "Selected Files" and update "BundleLock" to the Class found
 *   - Search for "GetDefaultNode"          using Options Search For: "Method",   "Selected Files" and update "DependencyGraph" to the Class found
 *     - The DependencyGraph Class takes a Type that you need to determine.
 *       - Double click on the search result "GetDefaultNode", this will take you to the method within the Class you just noted
 *       - Scroll up and select the class name on the class definition line
 *       - Press Ctrl+Shift+R to Analyze the usage 
 *       - Expand the Used By list and look for an instantiation of this class with and interface and update the GInterface for the DependencyGraph above
 */
namespace Aki.CustomBundles.Patches
{
    public class EasyAssetsPatch : GenericPatch<EasyAssetsPatch>
    {
        private static Type easyBundleType;
        private static string bundlesFieldName;

        public EasyAssetsPatch() : base(prefix: nameof(PatchPrefix)) { }

        protected override MethodBase GetTargetMethod()
        {
            // locate within the EFT.AbstractGame Assembly a Class that has a Property named "SameNameAsset"
            // This is the AssetBundle class that contains that property name  
            easyBundleType = PatcherConstants.TargetAssembly.GetTypes()
                .Single(type => type.IsClass && type.GetProperty("SameNameAsset") != null);
            // This is the EasyAssets class's property name that holds the AssetBundle collection 
            bundlesFieldName = easyBundleType.Name.ToLower() + "_0";

            // This targets the EasyAssets Class
            var targetType = PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType);
            // This returns the "method_0" method
            return AccessTools.GetDeclaredMethods(targetType).Single(IsTargetMethod);
        }

        // Locate the target class; must have more than two fields, must have a field named "Manifest" and must have a public "Create" method
        private static bool IsTargetType(Type type)
        {
            var fields = type.GetFields();

            if (fields.Length > 2)
                return false;

            if (!fields.Any(x => x.Name == "Manifest"))
                return false;

            return type.GetMethod("Create", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly) != null;
        }

        // Locate the target method; must have 5 arguments, 1st argument must be named "bundleLock", 2nd argument must be named "defaultKey" and the 5th argumanet must be named "shouldExclude"
        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length != 5 || parameters[0].Name != "bundleLock" || parameters[1].Name != "defaultKey" || parameters[4].Name != "shouldExclude") ? false : true;
        }

        // Execute this code instead of original
        static bool PatchPrefix(EasyAssets __instance, [CanBeNull] IBundleLock bundleLock, string defaultKey, string rootPath, string platformName, [CanBeNull] Func<string, bool> shouldExclude, ref Task __result)
        {
            __result = Init(__instance, bundleLock, defaultKey, rootPath, platformName, shouldExclude);
            return false;
        }

        public static async Task Init(EasyAssets __instance, [CanBeNull] IBundleLock bundleLock, string defaultKey, string rootPath, string platformName, [CanBeNull] Func<string, bool> shouldExclude)
        {
            var traverse = Traverse.Create(__instance);
            var path = rootPath.Replace("file:///", "").Replace("file://", "") + "/" + platformName + "/";
            
            var manifestLoading = AssetBundle.LoadFromFileAsync(path + platformName);
            await manifestLoading.Await();

            var assetBundle = manifestLoading.assetBundle;
            var assetLoading = assetBundle.LoadAllAssetsAsync();
            await assetLoading.Await();

            traverse.Field<AssetBundleManifest>("Manifest").Value = (AssetBundleManifest)assetLoading.allAssets[0];
            var manifest = traverse.Field<AssetBundleManifest>("Manifest").Value;

            //Add ModManifest
            var result = manifest.GetAllAssetBundles().ToList<string>();
            var resourcesModbundles = new List<string>();

            foreach (KeyValuePair<string, BundleInfo> kvp in Settings.bundles)
            {
                resourcesModbundles.Add(kvp.Key);
            }

            var bundleNames = result.Union(resourcesModbundles).ToList<string>().ToArray<string>();

            traverse.Field(bundlesFieldName).SetValue(Array.CreateInstance(easyBundleType, bundleNames.Length));

            if (bundleLock == null)
            {
                bundleLock = new BundleLock(int.MaxValue);
            }

            var bundles = traverse.Field(bundlesFieldName).GetValue<IEasyBundle[]>();

            for (var i = 0; i < bundleNames.Length; i++)
            {
                bundles[i] = (IEasyBundle)Activator.CreateInstance(easyBundleType, new object[] { bundleNames[i], path, manifest, bundleLock });
                await JobScheduler.Yield();
            }

            traverse.Field(bundlesFieldName).SetValue(bundles);
            traverse.Property<DependencyGraph>("System").Value = new DependencyGraph(bundles, defaultKey, shouldExclude);
        }
    }
}
