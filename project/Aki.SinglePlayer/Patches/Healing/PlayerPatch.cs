using System.Reflection;
using System.Threading.Tasks;
using EFT;
using Aki.Common.Utils.Patching;

namespace Aki.SinglePlayer.Patches.Healing
{
    class PlayerPatch : GenericPatch<PlayerPatch>
    {
        private static string _playerAccountId;

        public PlayerPatch() : base(postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        static async void PatchPostfix(Player __instance, Task __result)
        {
            if (_playerAccountId == null)
            {
                var backendSession = Utils.Config.BackEndSession;
                _playerAccountId = backendSession.Profile.AccountId;
            }

			if (__instance.Profile.AccountId != _playerAccountId)
			{
				return;
			}

            await __result;

            var listener = Utils.Healing.HealthListener.Instance;
            listener.Init(__instance.HealthController, true);
        }
    }
}
