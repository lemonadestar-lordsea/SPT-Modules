/* EasyBundlePatch.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Diz.DependencyManager;
using Aki.Common.Utils.Patching;
using Aki.CustomBundles.Utils;
using IEasyBundle = GInterface249; //Property: SameNameAsset 
using IBundleLock = GInterface250; //Property: IsLocked
using BindableState = GClass2161<Diz.DependencyManager.ELoadState>; //Construct method parameter: initialValue

/* Maintenance Tips
 * 
 * This patch is used to change behaivior of the "Diz Plugings - Achievements System"
 * The target class is an implementation of the bundle class called "EasyBundle", this patch will replace portions of the existing class and will not run the original code after.
 * 
 * Use dnSpy to find the correct GClass/GInterface/Property Name used within each patch.
 * 
 * dnSpy:
 *   - Open the un-obfuscated EFT CSharp Assemply "\EscapeFromTarkov_Data\Managed\Assembly-CSharp.dll"
 *   - Within the Assembly Expoler Tress, select the "Assembly-CSharp (0.0.0.0) file
 *   - Search for "SameNameAsset" using Options Search For: "Property", "Selected Files" and update "IEasyBundle" to the Interface found
 *   - Search for "IsLocked"      using Options Search For: "Property", "Selected Files" and update "IBundleLock" to the Interface found
 *   - Search for "initialValue"  using Options Search For: "Parameter", "Selected Files" and update "BindableState" to the Class found
 */
namespace Aki.CustomBundles.Patches
{
	public class EasyBundlePatch : GenericPatch<EasyBundlePatch>
	{
        public EasyBundlePatch() : base(prefix: nameof(PatchPrefix)) {}

		protected override MethodBase GetTargetMethod()
		{
            // Locate the first class constructor
            return PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType).GetConstructors()[0];
        }

        // Locate the target class; must have a Property named "SameNameAsset"
        private static bool IsTargetType(Type type)
        {
            return type.IsClass && type.GetProperty("SameNameAsset") != null;
        }

        // Execute this code instead of original
        static bool PatchPrefix(IEasyBundle __instance, string key, string rootPath, UnityEngine.AssetBundleManifest manifest, IBundleLock bundleLock)
		{
            var easyBundle = new EasyBundleHelper(__instance);
            easyBundle.Key = key;

            var path = rootPath + key;
            var bundle = (BundleInfo)null;

            if (Settings.bundles.TryGetValue(key, out bundle))
            {
                path = bundle.Path;
            }

            easyBundle.Path = path;
            easyBundle.KeyWithoutExtension = Path.GetFileNameWithoutExtension(key);

            var dependencyKeys = manifest.GetDirectDependencies(key);

            foreach (KeyValuePair<string, BundleInfo> kvp in Settings.bundles)
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
