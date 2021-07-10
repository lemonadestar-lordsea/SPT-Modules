using System.Linq;
using System.Reflection;
using Aki.Reflection.Patching;
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
            return typeof(MainMenuController)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .FirstOrDefault(IsTargetMethod);
        }

        private static void PrefixPatch(ref bool local)
        {
            local = false;
        }

        private static void PostfixPatch(ref bool ___bool_0)
        {
            ___bool_0 = true;
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length == 4
                    && parameters[0].Name == "local"
                    && parameters[1].Name == "weatherSettings"
                    && parameters[2].Name == "botsSettings"
                    && parameters[3].Name == "wavesSettings"
                    && parameters[0].ParameterType == typeof(bool));
        }
    }
}
