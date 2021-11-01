using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Healing
{
    public class MainMenuControllerPatch : Patch
    {
        static MainMenuControllerPatch()
        {
            _ = nameof(IHealthController.HydrationChangedEvent);
            _ = nameof(MainMenuController.HealthController);
        }

        public MainMenuControllerPatch() : base(T: typeof(MainMenuControllerPatch), postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethod("method_1", Constants.PrivateFlags);
        }

        private static void PatchPostfix(MainMenuController __instance)
        {
            var healthController = __instance.HealthController;
            var listener = Utils.Healing.HealthListener.Instance;
            listener.Init(healthController, false);
        }
    }
}
