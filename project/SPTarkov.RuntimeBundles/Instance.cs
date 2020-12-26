/* Instance.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using Aki.Common.Utils.Patching;
using Aki.RuntimeBundles.Patches;
using Aki.RuntimeBundles.Utils;
using UnityEngine;


namespace Aki.RuntimeBundles
{
    public class Instance : MonoBehaviour
    {
        private void Start()
		{
            Debug.LogError("Aki.RuntimeBundles: Loaded");

            new Settings(null, Config.BackendUrl);

            PatcherUtil.Patch<EasyAssetsPatch>();
            PatcherUtil.Patch<EasyBundlePatch>();
            PatcherUtil.Patch<BundleLoadPatch>();
        }
    }
}
