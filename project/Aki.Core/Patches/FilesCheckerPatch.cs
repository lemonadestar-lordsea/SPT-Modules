using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FilesChecker;
using Aki.Reflection.Patching;

namespace Aki.Core.Patches
{
    public class FakeFileCheckerResult : ICheckResult
    {
        public TimeSpan ElapsedTime { get; private set; }
        public Exception Exception { get; private set; }

        public FakeFileCheckerResult()
        {
            ElapsedTime = new TimeSpan();
            Exception = null;
        }
    }

    public class FilesCheckerPatch : GenericPatch<FilesCheckerPatch>
    {
        public FilesCheckerPatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(ICheckResult).Assembly.GetTypes().Single(x => x.Name == "ConsistencyController")
                .GetMethods().Single(x => x.Name == "EnsureConsistency" && x.ReturnType == typeof(Task<ICheckResult>));
        }

        private static bool PatchPrefix(ref object __result)
        {
            __result = Task.FromResult<ICheckResult>(new FakeFileCheckerResult());
            return false;
        }
    }
}
