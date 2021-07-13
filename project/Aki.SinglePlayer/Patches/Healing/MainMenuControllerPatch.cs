using System.Reflection;
using Aki.Reflection.Patching;
using MainMenuController = GClass1224;
using IHealthController = GInterface171;

namespace Aki.SinglePlayer.Patches.Healing
{
    public class MainMenuControllerPatch : GenericPatch<MainMenuControllerPatch>
    {
        static MainMenuControllerPatch()
        {
            _ = nameof(IHealthController.HydrationChangedEvent);
            _ = nameof(MainMenuController.HealthController);
        }

        public MainMenuControllerPatch() : base(postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static void PatchPostfix(MainMenuController __instance)
        {
            var healthController = __instance.HealthController;
            var listener = Utils.Healing.HealthListener.Instance;
            listener.Init(healthController, false);
        }
    }
}
