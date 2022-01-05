using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.MainMenu
{
    class AfkTimerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var type = PatchConstants.EftTypes.Single(x => x.Name == "MainApplication");
            return Array.Find(type.GetMethods(PatchConstants.PrivateFlags), IsTargetMethod);

        }

        private static bool IsTargetMethod(MethodInfo mi)
        {   // 16432 this is method_22
            var parameters = mi.GetParameters();
            return parameters.Length == 1
                && parameters[0].Name == "afkTimeOut";
        }

        [PatchPrefix]
        private static bool PatchPrefix(ref float afkTimeOut)
        {
            afkTimeOut = 21600f; // in seconds
            return true;
        }
    }
}   
