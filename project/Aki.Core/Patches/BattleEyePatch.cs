using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aki.Common.Utils.Patching;
using Aki.Core.Utils;

namespace Aki.Core.Patches
{
	public class BattleEyePatch : GenericPatch<BattleEyePatch>
	{
        public static PropertyInfo __property;
		
        public BattleEyePatch() : base(prefix: nameof(PatchPrefix))
		{
		}

		protected override MethodBase GetTargetMethod()
		{
			var methodName = "RunValidation";
			var flags = BindingFlags.Public | BindingFlags.Instance;
			var __type = PatcherConstants.TargetAssembly.GetTypes().Single(x => x.GetMethod(methodName, flags) != null);

            __property = __type.GetProperty("Succeed", flags);

            return __type.GetMethod(methodName, flags);
        }

        private static bool PatchPrefix(ref Task __result, object __instance)
		{
            __property.SetValue(__instance, ValidationUtil.Validate());
			__result = Task.CompletedTask;
			return false;
		}
	}
}
