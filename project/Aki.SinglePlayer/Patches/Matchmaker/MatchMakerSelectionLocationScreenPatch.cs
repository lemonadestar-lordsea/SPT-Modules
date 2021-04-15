using System.Reflection;
using EFT.UI;
using EFT.UI.Matchmaker;
using Aki.Common.Utils.Patching;

namespace Aki.SinglePlayer.Patches.Matchmaker
{
    class MatchMakerSelectionLocationScreenPatch : GenericPatch<MatchMakerSelectionLocationScreenPatch>
    {
        public MatchMakerSelectionLocationScreenPatch() : base(postfix: nameof(PatchPostfix))
        {
        }

        public static void PatchPostfix(DefaultUIButton ____readyButton)
        {
            ____readyButton.Interactable = false;
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MatchMakerSelectionLocationScreen).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}
