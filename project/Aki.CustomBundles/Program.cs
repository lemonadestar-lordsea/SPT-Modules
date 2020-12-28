/* Instance.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Craink
 * Merijn Hendriks
 */


using Aki.Common.Utils.Patching;
using Aki.CustomBundles.Patches;
using Aki.CustomBundles.Utils;
using UnityEngine;

namespace Aki.CustomBundles
{
    class Program
    {
        static void Main(string[] args)
		{
            Debug.LogError("Aki.CustomBundles: Loaded");

            new Settings(null, Config.BackendUrl);

            PatcherUtil.Patch<EasyAssetsPatch>();
            PatcherUtil.Patch<EasyBundlePatch>();
            PatcherUtil.Patch<BundleLoadPatch>();
        }
    }
}
