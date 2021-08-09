using Aki.Common;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Utils;
using EFT.UI;
using HarmonyLib;
using System.Linq;
using System.Reflection;
using Patch = Aki.Reflection.Patching.Patch;

namespace Aki.SinglePlayer.Patches.MainMenu
{
    public class VersionLabelPatch : Patch
    {
        private static string _versionLabel;

        public VersionLabelPatch() : base(T: typeof(VersionLabelPatch), postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.EftTypes
                .Single(x => x.GetField("Taxonomy", BindingFlags.Public | BindingFlags.Instance) != null)
                .GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
        }

        private static void PatchPostfix(object __result)
        {
            if (string.IsNullOrEmpty(_versionLabel))
            {
                var json = RequestHandler.GetJson("/singleplayer/settings/version");
                _versionLabel = Json.Deserialize<VersionResponse>(json).Version;
                Log.Info($"Server version: {_versionLabel}");
            }

            Traverse.Create(MonoBehaviourSingleton<PreloaderUI>.Instance).Field("_alphaVersionLabel").Property("LocalizationKey").SetValue("{0}");
            Traverse.Create(MonoBehaviourSingleton<PreloaderUI>.Instance).Field("string_1").SetValue(_versionLabel);
            Traverse.Create(__result).Field("Major").SetValue(_versionLabel);
        }

        private class VersionResponse
        {
            public string Version { get; set; }
        }
    }
}