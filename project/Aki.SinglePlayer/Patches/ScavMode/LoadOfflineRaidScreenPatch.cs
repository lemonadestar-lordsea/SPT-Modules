using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using EFT;
using EFT.UI.Matchmaker;
using EFT.UI.Screens;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils.Reflection;
using MenuController = GClass1253;
using WeatherSettings = GStruct93;
using BotsSettings = GStruct234;
using WavesSettings = GStruct94;

namespace Aki.SinglePlayer.Patches.ScavMode
{
    using OfflineRaidAction = Action<bool, WeatherSettings, BotsSettings, WavesSettings>;

    public class LoadOfflineRaidScreenPatch : GenericPatch<LoadOfflineRaidScreenPatch>
    {
        public LoadOfflineRaidScreenPatch() : base(transpiler: nameof(PatchTranspiler))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MenuController).GetNestedTypes(BindingFlags.NonPublic)
                .Single(x => x.Name == "Class848")
                .GetMethod("method_2", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var index = 26;
            var callCode = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LoadOfflineRaidScreenPatch), "LoadOfflineRaidScreenForScav"));

            codes[index].opcode = OpCodes.Nop;
            codes[index + 1] = callCode;
            codes.RemoveAt(index + 2);

            return codes.AsEnumerable();
        }

        private static MenuController GetMenuController()
        {
            return PrivateValueAccessor.GetPrivateFieldValue(typeof(MainApplication), $"{typeof(GClass1253).Name.ToLower()}_0",
                                                             ClientAppUtils.GetMainApp()) as MenuController;
        }

        // Refer to MatchmakerOfflineRaid's subclass's OnShowNextScreen action definitions if these structs numbers change.
        public static void LoadOfflineRaidNextScreen(bool local, WeatherSettings weatherSettings, BotsSettings botsSettings, WavesSettings wavesSettings)
        {
            var menuController = GetMenuController();

            if (menuController.SelectedLocation.Id == "laboratory")
            {
                wavesSettings.IsBosses = true;
            }

            SetMenuControllerFieldValue(menuController, "bool_0", local);
            SetMenuControllerFieldValue(menuController, $"{typeof(GStruct93).Name.ToLower()}_0", weatherSettings);
            SetMenuControllerFieldValue(menuController, $"{typeof(GStruct94).Name.ToLower()}_0", wavesSettings);
            SetMenuControllerFieldValue(menuController, $"{typeof(GStruct234).Name.ToLower()}_0", botsSettings);

            // load ready screen method
            typeof(MenuController).GetMethod("method_40", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(menuController, null);
        }

        public static void LoadOfflineRaidScreenForScav()
        {
            var menuController = (object)GetMenuController();
            var gclass = new MatchmakerOfflineRaid.GClass2112();

            gclass.OnShowNextScreen += LoadOfflineRaidNextScreen;

            // ready method
            gclass.OnShowReadyScreen += (OfflineRaidAction)Delegate.CreateDelegate(typeof(OfflineRaidAction), menuController, "method_61");
            gclass.ShowScreen(EScreenState.Queued);
        }

        private static void SetMenuControllerFieldValue(MenuController instance, string fieldName, object value)
        {
            PrivateValueAccessor.SetPrivateFieldValue(typeof(MenuController), fieldName, instance, value);
        }
    }
}
