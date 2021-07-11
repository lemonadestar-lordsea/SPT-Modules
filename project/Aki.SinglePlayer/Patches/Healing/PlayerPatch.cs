using System.Reflection;
using System.Threading.Tasks;
using EFT;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;

namespace Aki.SinglePlayer.Patches.Healing
{
    public class PlayerPatch : GenericPatch<PlayerPatch>
    {
        private static string _accountId;

        public PlayerPatch() : base(postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static async void PatchPostfix(Player __instance, Task __result)
        {
            if (_accountId == null)
            {
                _accountId = Constants.BackEndSession.Profile.AccountId;
            }

            if (__instance.Profile.AccountId != _accountId)
            {
                return;
            }

            await __result;

            var listener = Utils.Healing.HealthListener.Instance;
            listener.Init(__instance.HealthController, true);
        }
    }
}
