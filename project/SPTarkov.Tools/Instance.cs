/* Instance.cs
 * License: NCSA Open Source License
 * 
 * Copyright: SPT AKI
 * AUTHORS:
 * Ginja
 */


using SPTarkov.Common.Utils.Patching;
using SPTarkov.Tools.Patches;
using UnityEngine;

namespace SPTarkov.Tools
{
	public class Instance : MonoBehaviour
	{
		private void Start()
		{
			//UnityEngine.Debug.LogError("SPTarkov.Tools: Loaded");

            //PatcherUtil.Patch<CoordinatesPatch>();
        }
	}
}
