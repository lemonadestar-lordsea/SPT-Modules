using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Healing
{
    /// <summary>
    /// We want to alter the _raidsettings.raidmode to online prior to calling this line of code:
    /// Class1049 @class = Class1049.smethod_0(_backEnd, profileId, savageProfile, location, exitStatus, exitTime, _raidSettings.RaidMode);
    /// The post-raid heal page only shows when raidmode = online
    /// </summary>
    public class PostRaidHealScreenPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // method_44 as of 18913
            return typeof(MainApplication).GetMethods(PatchConstants.PrivateFlags).Single(IsTargetMethod);
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length > 5
                && parameters[0].Name == "profileId"
                && parameters[1].Name == "savageProfile"
                && parameters[2].Name == "location"
                && parameters[3].Name == "exitStatus"
                && parameters[4].Name == "exitTime") ? true : false;
        }

        [PatchPrefix]
        private static bool PatchPrefix(
            MainApplication __instance,
            RaidSettings ____raidSettings)
        {
            Logger.LogInfo($"pre prefix {____raidSettings.RaidMode}");
            ____raidSettings.RaidMode = ERaidMode.Local;
            Logger.LogInfo($"post prefix{____raidSettings.RaidMode}");

            return true;
        }

        [PatchPostfix]
        private static void PatchPostfix(
            MainApplication __instance,
            RaidSettings ____raidSettings)
        {
            Logger.LogInfo($"pre postfix {____raidSettings.RaidMode}");
            ____raidSettings.RaidMode = ERaidMode.Online;
            Logger.LogInfo($"post postfix {____raidSettings.RaidMode}");
        }
    }
}
