using UnityEngine;
using Aki.Core.Patches;

namespace Aki.Core
{
	class Program
	{
		static void Main(string[] args)
		{
            Debug.LogError("Aki.Core: Loaded");

            new BattlEyePatch().Apply();
            new SslCertificatePatch().Apply();
            new UnityWebRequestPatch().Apply();
        }
	}
}
