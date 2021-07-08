using Aki.Common.Utils;
using Aki.Core.Patches;

namespace Aki.Core
{
	class Program
	{
		static void Main(string[] args)
		{
            Log.Info("Aki.Core: Loaded");

            new BattlEyePatch().Apply();
            new SslCertificatePatch().Apply();
            new UnityWebRequestPatch().Apply();
        }
	}
}
