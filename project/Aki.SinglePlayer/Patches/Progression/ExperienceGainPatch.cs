using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT.UI.SessionEnd;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class ExperienceGainPatch : Patch
    {
        public ExperienceGainPatch() : base(T: typeof(ExperienceGainPatch), prefix: nameof(PrefixPatch), postfix: nameof(PostfixPatch))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(SessionResultExperienceCount).GetMethods(Constants.PrivateFlags).FirstOrDefault(IsTargetMethod);
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length == 3
                && parameters[0].Name == "profile"
                && parameters[1].Name == "isLocal"
                && parameters[2].Name == "exitStatus"
                && parameters[1].ParameterType == typeof(bool));
        }

        private static void PrefixPatch(ref bool isLocal)
        {
            isLocal = false;
        }

        private static void PostfixPatch(ref bool isLocal)
        {
            isLocal = true;
        }
    }
}
