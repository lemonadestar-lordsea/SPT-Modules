using Aki.Common.Http;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System;
using System.Linq;
using System.Reflection;

namespace Aki.Custom.Patches
{
    class AfkTimerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var type = PatchConstants.EftTypes.Single(x => x.Name == "MainApplication");
            return Array.Find(type.GetMethods(PatchConstants.PrivateFlags), IsTargetMethod);
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {   
            var parameters = mi.GetParameters();
            return parameters.Length == 1
                && parameters[0].Name == "afkTimeOut";
        }

        [PatchPrefix] 
        private static bool PatchPrefix(ref float afkTimeOut)
        {
            var json = RequestHandler.GetJson("/singleplayer/settings/afkTimeOut");
            var isParsable = float.TryParse(json, out afkTimeOut);

            // time is in seconds
            afkTimeOut = isParsable ? afkTimeOut : 86400;
            return true;
        }
    }
}   
