using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aki.Common.Utils.Patching;
using Aki.Core.Utils;

namespace Aki.Core.Patches
{
	public class BattlEyePatch : GenericPatch<BattlEyePatch>
	{
        public static PropertyInfo __property;
		
        public BattlEyePatch() : base(prefix: nameof(PatchPrefix))
		{
		}

		protected override MethodBase GetTargetMethod()
		{
			var methodName = "RunValidation";
			var flags = BindingFlags.Public | BindingFlags.Instance;
			var type = PatcherConstants.TargetAssembly.GetTypes().Single(x => x.GetMethod(methodName, flags) != null);

            __property = type.GetProperty("Succeed", flags);

            return type.GetMethod(methodName, flags);
        }

        private static bool PatchPrefix(ref Task __result, object __instance)
		{
            __property.SetValue(__instance, ValidationUtil.Validate());
			__result = Task.CompletedTask;
			return false;
		}
	}
}
