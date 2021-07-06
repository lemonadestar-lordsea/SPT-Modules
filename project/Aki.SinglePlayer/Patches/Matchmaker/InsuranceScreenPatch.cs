using System.Linq;
using System.Reflection;
using Aki.Common.Utils.Patching;
using MainMenuController = GClass1253;

namespace Aki.SinglePlayer.Patches.Matchmaker
{
    class InsuranceScreenPatch : GenericPatch<InsuranceScreenPatch>
    {
        static InsuranceScreenPatch()
        {
            _ = nameof(MainMenuController.InventoryController);
        }

        public InsuranceScreenPatch() : base(prefix: nameof(PrefixPatch), postfix: nameof(PostfixPatch))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethod("method_60");
        }

        static void PrefixPatch(ref bool local)
        {
            local = false;
        }

        static void PostfixPatch(ref bool ___bool_0)
        {
            ___bool_0 = true;
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(IsTargetMethod);    // controller contains 2 methods with same signature. Usually target method is first of them.
        }
    }
}
