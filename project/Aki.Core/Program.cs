/* Instance.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using UnityEngine;
using Aki.Core.Patches;
using Aki.Common.Utils.Patching;

namespace Aki.Core
{
	class Program
	{
		static void Main(string[] args)
		{
            Debug.LogError("Aki.Core: Loaded");

            PatcherUtil.Patch<BattleEyePatch>();
            PatcherUtil.Patch<SslCertificatePatch>();
            PatcherUtil.Patch<UnityWebRequestPatch>();
        }
	}
}
