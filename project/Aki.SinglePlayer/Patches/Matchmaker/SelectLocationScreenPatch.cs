using System.Reflection;
using EFT.UI;
using EFT.UI.Matchmaker;
using Aki.Reflection.Patching;

namespace Aki.SinglePlayer.Patches.Matchmaker
{
    public class SelectLocationScreenPatch : GenericPatch<SelectLocationScreenPatch>
    {
        public SelectLocationScreenPatch() : base(postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MatchMakerSelectionLocationScreen).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static void PatchPostfix(DefaultUIButton ____readyButton)
        {
            ____readyButton.Interactable = false;
        }
    }
}
