using System.Reflection;
using System.Threading.Tasks;
using EFT;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;

namespace Aki.SinglePlayer.Patches.Healing
{
    public class PlayerPatch : GenericPatch<PlayerPatch>
    {
        public PlayerPatch() : base(postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod(Profile ___profile_0)
        {
            return typeof(Player).GetMethod("Init", Constants.PrivateFlags);
        }

        private static async void PatchPostfix(Task __result)
        {
            await __result;

            var listener = Utils.Healing.HealthListener.Instance;
            listener.Init(__instance.HealthController, true);
        }
    }
}
