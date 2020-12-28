/* Instance.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Craink
 * Merijn Hendriks
 */


using Aki.Common.Utils.Patching;
using Aki.Bundles.Patches;
using Aki.Bundles.Utils;
using UnityEngine;

namespace Aki.Bundles
{
    class Program
    {
        static void Main(string[] args)
		{
            Debug.LogError("Aki.Bundles: Loaded");

            new Settings(null, Config.BackendUrl);

            PatcherUtil.Patch<EasyAssetsPatch>();
            PatcherUtil.Patch<EasyBundlePatch>();
            PatcherUtil.Patch<BundleLoadPatch>();
        }
    }
}
