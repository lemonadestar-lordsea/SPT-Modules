using System.Reflection;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils.Reflection;

namespace Aki.SinglePlayer.Patches.RaidFix
{
    public class OnShellEjectEventPatch : GenericPatch<OnShellEjectEventPatch>
    {
        public OnShellEjectEventPatch() : base(prefix: nameof(PatchPrefix)) { }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.FirearmControllerType.GetMethod("OnShellEjectEvent");
        }

        static bool PatchPrefix(object __instance)
        {
            var weaponController = PrivateValueAccessor.GetPrivateFieldValue(PatcherConstants.FirearmControllerType, PatcherConstants.WeaponControllerFieldName, __instance);
            return (weaponController.GetType().GetField("RemoveFromChamberResult").GetValue(weaponController) == null) ? false : true;
        }
    }
}
