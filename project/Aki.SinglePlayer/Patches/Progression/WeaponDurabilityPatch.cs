using System.Linq;
using System.Reflection;
using UnityEngine;
using EFT;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Utils.Progression;
using AmmoInfo = GClass1746;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class WeaponDurabilityPatch : GenericPatch<WeaponDurabilityPatch>
    {
        static WeaponDurabilityPatch()
        {
            _ = nameof(AmmoInfo.AmmoLifeTimeSec);
        }

        public WeaponDurabilityPatch() : base(postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player.FirearmController).GetMethods(Constants.PrivateFlags).Single(IsTargetMethod);
        }

        private static bool IsTargetMethod(MethodInfo methodInfo)
        {
            if (methodInfo.IsVirtual)
            {
                return false;
            }

            var parameters = methodInfo.GetParameters();
            var methodBody = methodInfo.GetMethodBody();

            if (parameters.Length != 1
                || parameters[0].ParameterType != typeof(AmmoInfo)
                || parameters[0].Name != "ammo")
            {
                return false;
            }

            return methodBody.LocalVariables.Any(x => x.LocalType == typeof(Vector3));
        }

        public static void PatchPostfix(Player.FirearmController __instance, AmmoInfo ammo)
        {
            if (!DurabilityConfig.Enabled)
            {
                return;
            }

            var item = __instance.Item;
            var durability = item.Repairable.Durability;
            var deterioration = ammo.Deterioration;
            var operatingResource = (item.Template.OperatingResource > 0) ? item.Template.OperatingResource : 1;

            if (durability <= 0f)
            {
                return;
            }

            durability -= item.Repairable.MaxDurability / operatingResource * deterioration;
            item.Repairable.Durability = (durability > 0) ? durability : 0;
        }
    }
}
