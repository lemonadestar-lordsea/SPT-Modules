using Aki.Common.Utils;

namespace Aki.Core
{
	class Program
	{
		static void Main(string[] args)
		{
            Log.Info("Loading: Aki.Core");
			PatchManager.Patches.EnableAll();
		}
	}
}
