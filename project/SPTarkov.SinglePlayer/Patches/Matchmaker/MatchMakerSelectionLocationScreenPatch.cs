using System.Reflection;
using EFT.UI;
using EFT.UI.Matchmaker;
using SPTarkov.Common.Utils.Patching;

namespace SPTarkov.SinglePlayer.Patches.Matchmaker
{
    class MatchMakerSelectionLocationScreenPatch : GenericPatch<MatchMakerSelectionLocationScreenPatch>
    {
        public MatchMakerSelectionLocationScreenPatch() : base(postfix: nameof(PatchPostfix))
        {
        }

        public static void PatchPostfix(UIButtonSpawner ____readyButton)
        {
            ____readyButton.SpawnedObject.gameObject.SetActive(false);
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MatchMakerSelectionLocationScreen).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}