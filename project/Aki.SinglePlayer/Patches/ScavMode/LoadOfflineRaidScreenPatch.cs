using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using EFT;
using EFT.UI.Matchmaker;
using EFT.UI.Screens;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using MenuController = GClass1253;
using WeatherSettings = GStruct93;
using BotsSettings = GStruct234;
using WavesSettings = GStruct94;

namespace Aki.SinglePlayer.Patches.ScavMode
{
    using OfflineRaidAction = Action<bool, WeatherSettings, BotsSettings, WavesSettings>;

    public class LoadOfflineRaidScreenPatch : GenericPatch<LoadOfflineRaidScreenPatch>
    {
        private static MethodInfo _onReadyScreenMethod;
        private static FieldInfo _weatherSettingsField;
        private static FieldInfo _botsSettingsField;
        private static FieldInfo _waveSettingsField;
        private static FieldInfo _isLocalField;
        private static FieldInfo _menuControllerField;

        static LoadOfflineRaidScreenPatch()
        {
            _ = nameof(MenuController.InventoryController);
            _ = nameof(WeatherSettings.IsRandomWeather);
            _ = nameof(BotsSettings.IsScavWars);
            _ = nameof(WavesSettings.IsBosses);
        }

        public LoadOfflineRaidScreenPatch() : base(transpiler: nameof(PatchTranspiler))
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            var menuControllerType = typeof(MenuController);

            _onReadyScreenMethod = menuControllerType.GetMethod("method_40", flags);
            _weatherSettingsField = menuControllerType.GetField($"{nameof(GStruct93).ToLower()}_0", flags);
            _botsSettingsField = menuControllerType.GetField($"{nameof(GStruct94).ToLower()}_0", flags);
            _waveSettingsField = menuControllerType.GetField($"{nameof(GStruct234).ToLower()}_0", flags);
            _isLocalField = menuControllerType.GetField("bool_0", flags);
            _menuControllerField = typeof(MainApplication).GetField($"{nameof(GClass1253).ToLower()}_0", flags);
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MenuController).GetNestedTypes(BindingFlags.NonPublic)
                .Single(x => x.Name == "Class848")
                .GetMethod("method_2", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        private static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions)
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
            return _menuControllerField.GetValue(ClientAppUtils.GetMainApp()) as MenuController;                                  
        }

        private static void LoadOfflineRaidNextScreen(bool local, WeatherSettings weatherSettings, BotsSettings botsSettings, WavesSettings wavesSettings)
        {
            var menuController = GetMenuController();

            if (menuController.SelectedLocation.Id == "laboratory")
            {
                wavesSettings.IsBosses = true;
            }

            // set offline raid values
            _weatherSettingsField.SetValue(menuController, weatherSettings);
            _botsSettingsField.SetValue(menuController, wavesSettings);
            _waveSettingsField.SetValue(menuController, botsSettings);
            _isLocalField.SetValue(menuController, local);

            // load ready screen method
            _onReadyScreenMethod.Invoke(menuController, null);
        }

        private static void LoadOfflineRaidScreenForScav()
        {
            var menuController = (object)GetMenuController();
            var gclass = new MatchmakerOfflineRaid.GClass2112();

            gclass.OnShowNextScreen += LoadOfflineRaidNextScreen;

            // ready method
            gclass.OnShowReadyScreen += (OfflineRaidAction)Delegate.CreateDelegate(typeof(OfflineRaidAction), menuController, "method_61");
            gclass.ShowScreen(EScreenState.Queued);
        }
    }
}
