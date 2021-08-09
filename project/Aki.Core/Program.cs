using Aki.Common;
using Aki.Core.Patches;

namespace Aki.Core
{
	class Program
	{
		static void Main(string[] args)
		{
            Log.Info("Loading: Aki.Core");

			var patcher = new PatchManager();
            patcher.RunPatches();
		}
	}
}
