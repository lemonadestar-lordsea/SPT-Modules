using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using EFT.InventoryLogic;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Quests
{
    public class BeaconPatch : ModulePatch
    {
        public BeaconPatch() : base(T: typeof(BeaconPatch), prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethods(Constants.PrivateFlags).Single(IsTargetMethod);
        }

        private bool IsTargetMethod(MethodInfo method)
        {
            if (!method.IsVirtual)
            {
                return false;
            }

            var parameters = method.GetParameters();

            return (parameters.Length == 2
                && parameters[0].Name == "item"
                && parameters[1].Name == "zone"
                && parameters[0].ParameterType == typeof(Item)
                && parameters[1].ParameterType == typeof(string));
        }

        private static bool PatchPrefix(Player __instance, Item item, string zone)
        {
            __instance.Profile.ItemDroppedAtPlace(item.TemplateId, zone);
            return false;
        }
    }
}
