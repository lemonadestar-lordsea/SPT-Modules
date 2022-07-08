using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Healing
{
    /// <summary>
    /// We need to alter Class1049.smethod_0().
    /// Set the passed in ERaidMode to online, this ensures the heal screen shows.
    /// It cannot be changed in the calling method as doing so causes the post-raid exp display to remain at 0
    /// </summary>
    public class PostRaidHealScreenPatch2 : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            // method_44 as of 18913
            var typeWeWant = PatchConstants.EftTypes.Single(x => x.Name == "Class1049");
            var methods = typeWeWant.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
            return methods.Single(x => x.Name == "smethod_0");
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
            ref ERaidMode raidMode)
        {
            Logger.LogInfo($"pre prefix {raidMode}");
            raidMode = ERaidMode.Online;
            Logger.LogInfo($"post prefix {raidMode}");

            return true;
        }
    }
}
