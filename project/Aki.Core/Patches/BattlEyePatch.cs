using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.Core.Utils;

namespace Aki.Core.Patches
{
    public class BattlEyePatch : GenericPatch<BattlEyePatch>
    {
        public BattlEyePatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            var methodName = "RunValidation";
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var type = Constants.EftTypes.Single(x => x.GetMethod(methodName, flags) != null);

            return type.GetMethod(methodName, flags);
        }

        private static bool PatchPrefix(ref Task __result, ref bool ___bool_0)
        {
            ___bool_0 = ValidationUtil.Validate();
            __result = Task.CompletedTask;
            return false;
        }
    }
}
