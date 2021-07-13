using System.Reflection;
using EFT.UI;
using EFT.UI.Matchmaker;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;

namespace Aki.SinglePlayer.Patches.Matchmaker
{
    public class SelectLocationScreenPatch : GenericPatch<SelectLocationScreenPatch>
    {
        public SelectLocationScreenPatch() : base(postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MatchMakerSelectionLocationScreen).GetMethod("Awake", Constants.PrivateFlags);
        }

        private static void PatchPostfix(DefaultUIButton ____readyButton)
        {
            ____readyButton.Interactable = false;
        }
    }
}
