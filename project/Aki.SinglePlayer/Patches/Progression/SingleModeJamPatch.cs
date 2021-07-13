using System;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Utils.Progression;
using static EFT.Player;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class SingleModeJamPatch : GenericPatch<SingleModeJamPatch>
    {
        private const string _targetMethodName = "PrepareShot";
        private static MethodInfo _onFireEventMethod;

        public SingleModeJamPatch() : base(postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            var targetType = Constants.EftTypes.Single(IsTargetType);
            _onFireEventMethod = targetType.GetMethod("OnFireEvent", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            return targetType.GetMethod(_targetMethodName, Constants.PrivateFlags);
        }

        private static bool IsTargetType(Type type)
        {
            if (type.DeclaringType == null
                || type.DeclaringType.DeclaringType == null
                || type.DeclaringType.Name != nameof(FirearmController)
                || type.DeclaringType.DeclaringType.Name != nameof(Player))
            {
                return false;
            }

            var prepareShotMethod = type.GetMethod(_targetMethodName, Constants.PrivateFlags);

            if (prepareShotMethod == null)
            {
                return false;
            }
            
            return prepareShotMethod.GetMethodBody().LocalVariables.Any(x => x.LocalType == typeof(Weapon.EMalfunctionState));
        }

        public static void PatchPostfix(object __instance, Weapon ___weapon_0, FirearmsAnimator ___firearmsAnimator_0, FirearmController ___firearmController_0)
        {
            if (!DurabilityConfig.Enabled || ___weapon_0.MalfunctionState != Weapon.EMalfunctionState.Jam)
            {
                return;
            }

            _onFireEventMethod.Invoke(__instance, new object[] { });
            ___firearmsAnimator_0.Animator.Play("JAM", 1, 0f);
            ___firearmController_0.EmitEvents();
        }
    }
}
