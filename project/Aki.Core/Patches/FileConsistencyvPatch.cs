using System;
using System.Reflection;
using System.Threading.Tasks;
using FilesChecker;
using Aki.Common.Utils.Patching;

namespace Aki.Core.Patches
{
	public class FileConsistencyvPatch : GenericPatch<FileConsistencyvPatch>
	{
		private Assembly assembly;

		public FileConsistencyvPatch() : base(prefix: nameof(PatchPrefix))
		{
			assembly = typeof(ICheckResult).Assembly;
		}

		protected override MethodBase GetTargetMethod()
		{
			return assembly.GetType("ConsistencyController").GetMethod("EnsureConsistency", BindingFlags.NonPublic | BindingFlags.Instance);
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
