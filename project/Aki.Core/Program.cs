using Aki.Common;
using Aki.Core.Patches;

namespace Aki.Core
{
	class Program
	{
		static void Main(string[] args)
		{
            Log.Info("Loading: Aki.Core");

            new FilesCheckerPatch().Apply();
            new BattlEyePatch().Apply();
            new SslCertificatePatch().Apply();
            new UnityWebRequestPatch().Apply();
        }
	}
}
