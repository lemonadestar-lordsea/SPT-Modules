using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using System.Reflection;
using System.Threading.Tasks;

namespace Aki.SinglePlayer.Patches.Healing
{
    public class PlayerPatch : Patch
    {
        public PlayerPatch() : base(T: typeof(PlayerPatch), postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", Constants.PrivateFlags);
        }

        private static async void PatchPostfix(Task __result, Player __instance, Profile profile)
        {
            await __result;

            if (profile != null && profile.Id.StartsWith("pmc"))
            {
                var listener = Utils.Healing.HealthListener.Instance;
                listener.Init(__instance.HealthController, true);
            }
        }
    }
}
