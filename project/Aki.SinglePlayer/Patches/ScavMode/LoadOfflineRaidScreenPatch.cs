using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using EFT;
using EFT.UI.Matchmaker;
using EFT.UI.Screens;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using OfflineRaidAction = System.Action;

// DON'T FORGET TO UPDATE REFERENCES IN CONSTRUCTOR
// AND IN THE LoadOfflineRaidScreenForScavs METHOD AS WELL

namespace Aki.SinglePlayer.Patches.ScavMode
{
    public class LoadOfflineRaidScreenPatch : ModulePatch
    {
        private static readonly MethodInfo _onReadyScreenMethod;
        private static readonly FieldInfo _weatherSettingsField;
        private static readonly FieldInfo _botsSettingsField;
        private static readonly FieldInfo _waveSettingsField;
        private static readonly FieldInfo _isLocalField;
        private static readonly FieldInfo _menuControllerField;

        static LoadOfflineRaidScreenPatch()
        {
            _ = nameof(MainMenuController.InventoryController);
            _ = nameof(WeatherSettings.IsRandomWeather);
            _ = nameof(BotsSettings.IsScavWars);
            _ = nameof(WavesSettings.IsBosses);

            var menuControllerType = typeof(MainMenuController);

            _onReadyScreenMethod = menuControllerType.GetMethod("method_39", PatchConstants.PrivateFlags);
            _isLocalField = menuControllerType.GetField("bool_0", PatchConstants.PrivateFlags);
            _menuControllerField = typeof(MainApplication).GetFields(PatchConstants.PrivateFlags).FirstOrDefault(x => x.FieldType == typeof(MainMenuController));

            if (_menuControllerField == null)
            {
                Logger.LogError("LoadOfflineRaidScreenPatch() menuControllerField is null and could not be found in MainApplication class");
            }

            foreach (var field in menuControllerType.GetFields(PatchConstants.PrivateFlags))
            {
                if (field.FieldType == typeof(WeatherSettings))
                {
                    _weatherSettingsField = field;
                    continue;
                }

                if (field.FieldType == typeof(WavesSettings))
                {
                    _waveSettingsField = field;
                    continue;
                }

                if (field.FieldType == typeof(BotsSettings))
                {
                    _botsSettingsField = field;
                }
            }
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetNestedTypes(BindingFlags.NonPublic)
                .Single(x => x.IsNested && x.GetField("selectLocationScreenController", BindingFlags.Public | BindingFlags.Instance) != null)
                .GetMethod("method_2", PatchConstants.PrivateFlags);
        }

        [PatchTranspiler]
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

        private static MainMenuController GetMenuController()
        {
            return _menuControllerField.GetValue(ClientAppUtils.GetMainApp()) as MainMenuController;
        }

        private static void LoadOfflineRaidNextScreen()
        {
            var menuController = GetMenuController();

            var raidSettings = Traverse.Create(menuController).Field("raidSettings_0").GetValue<RaidSettings>();

            if (raidSettings.SelectedLocation.Id == "laboratory")
            {
                raidSettings.WavesSettings.IsBosses = true;
            }

            // set offline raid values
            _weatherSettingsField.SetValue(menuController, raidSettings.TimeAndWeather);
            _botsSettingsField.SetValue(menuController, raidSettings.BotSettings);
            _waveSettingsField.SetValue(menuController, raidSettings.WavesSettings);
            _isLocalField.SetValue(menuController, raidSettings.Local);

            // load ready screen method
            _onReadyScreenMethod.Invoke(menuController, null);
        }

        private static void LoadOfflineRaidScreenForScav()
        {
            var profile = PatchConstants.BackEndSession.Profile;
            var menuController = (object)GetMenuController();
            var raidSettings = Traverse.Create(menuController).Field("raidSettings_0").GetValue<RaidSettings>();
            var gclass = new MatchmakerOfflineRaidScreen.GClass2503(profile?.Info, ref raidSettings);

            gclass.OnShowNextScreen += LoadOfflineRaidNextScreen;

            // ready method
            gclass.OnShowReadyScreen += (OfflineRaidAction)Delegate.CreateDelegate(typeof(OfflineRaidAction), menuController, "method_63");
            gclass.ShowScreen(EScreenState.Queued);
        }
    }
}