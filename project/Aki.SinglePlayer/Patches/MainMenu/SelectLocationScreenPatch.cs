using System.Reflection;
using EFT.UI;
using EFT.UI.Matchmaker;
using Aki.Reflection.Utils;

namespace Aki.SinglePlayer.Patches.MainMenu
{
    public class SelectLocationScreenPatch : Patch
    {
        public SelectLocationScreenPatch() : base(T: typeof(SelectLocationScreenPatch), postfix: nameof(PatchPostfix))
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
