using Aki.Core.Utils;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Aki.Core.Patches
{
    public class BattlEyePatch : Patch
    {
        public BattlEyePatch() : base(T: typeof(BattlEyePatch), prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            var methodName = "RunValidation";
            var flags = BindingFlags.Public | BindingFlags.Instance;

            return Constants.EftTypes.Single(x => x.GetMethod(methodName, flags) != null)
                .GetMethod(methodName, flags);
        }

        private static bool PatchPrefix(ref Task __result, ref bool ___bool_0)
        {
            ___bool_0 = ValidationUtil.Validate();
            __result = Task.CompletedTask;
            return false;
        }
    }
}
