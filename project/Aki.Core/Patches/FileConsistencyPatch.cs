using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FilesChecker;
using Aki.Common.Utils.Patching;

namespace Aki.Core.Patches
{
	public class FileConsistencyPatch : GenericPatch<FileConsistencyPatch>
	{
		public FileConsistencyPatch() : base(prefix: nameof(PatchPrefix))
		{
		}

		protected override MethodBase GetTargetMethod()
		{
			return Assembly.GetAssembly(typeof(ICheckResult))
				.GetTypes().Single(x => x.Name == "ConsistencyController")
				.GetMethod("EnsureConsistency", BindingFlags.Public | BindingFlags.Instance);
		}

		private static bool PatchPrefix(ref Task<ICheckResult> __result)
		{
			__result = Task.FromResult<ICheckResult>(new FakeResult());
			return false;
		}
	}

	class FakeResult : ICheckResult
	{
		public TimeSpan ElapsedTime { get; private set; }
		public Exception Exception { get; private set; }

		public FakeResult()
		{
			ElapsedTime = new TimeSpan(5);
			Exception = null;
		}
	}
}
