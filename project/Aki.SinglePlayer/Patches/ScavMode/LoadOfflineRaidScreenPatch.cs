﻿using Aki.Reflection.Utils;
using EFT;
using EFT.UI.Matchmaker;
using EFT.UI.Screens;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BotsSettings = GStruct232;
using MenuController = GClass1453;
using Patch = Aki.Reflection.Patching.Patch;
using WavesSettings = GStruct92;
using WeatherSettings = GStruct91;
using OfflineRaidAction = System.Action<bool, GStruct91, GStruct232, GStruct92>;

// DON'T FORGET TO UPDATE REFERENCES IN CONSTRUCTOR

namespace Aki.SinglePlayer.Patches.ScavMode
{
    public class LoadOfflineRaidScreenPatch : Patch
    {
        private static readonly MethodInfo _onReadyScreenMethod;
        private static readonly FieldInfo _weatherSettingsField;
        private static readonly FieldInfo _botsSettingsField;
        private static readonly FieldInfo _waveSettingsField;
        private static readonly FieldInfo _isLocalField;
        private static readonly FieldInfo _menuControllerField;

        static LoadOfflineRaidScreenPatch()
        {
            _ = nameof(MenuController.InventoryController);
            _ = nameof(WeatherSettings.IsRandomWeather);
            _ = nameof(BotsSettings.IsScavWars);
            _ = nameof(WavesSettings.IsBosses);

            var menuControllerType = typeof(MenuController);

            _onReadyScreenMethod = menuControllerType.GetMethod("method_40", Constants.PrivateFlags);
            _weatherSettingsField = menuControllerType.GetField($"{nameof(GStruct91).ToLowerInvariant()}_0", Constants.PrivateFlags);
            _botsSettingsField = menuControllerType.GetField($"{nameof(GStruct92).ToLowerInvariant()}_0", Constants.PrivateFlags);
            _waveSettingsField = menuControllerType.GetField($"{nameof(GStruct232).ToLowerInvariant()}_0", Constants.PrivateFlags);
            _isLocalField = menuControllerType.GetField("bool_0", Constants.PrivateFlags);
            _menuControllerField = typeof(MainApplication).GetField($"{nameof(GClass1453).ToLowerInvariant()}_0", Constants.PrivateFlags);
        }

        public LoadOfflineRaidScreenPatch() : base(T: typeof(LoadOfflineRaidScreenPatch), transpiler: nameof(PatchTranspiler))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MenuController).GetNestedTypes(BindingFlags.NonPublic)
                .Single(x => x.IsNested && x.GetField("selectLocationScreenController", BindingFlags.Public | BindingFlags.Instance) != null)
                .GetMethod("method_2", Constants.PrivateFlags);
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
            var gclass = new MatchmakerOfflineRaid.GClass2324();

            gclass.OnShowNextScreen += LoadOfflineRaidNextScreen;

            // ready method
            gclass.OnShowReadyScreen += (OfflineRaidAction)Delegate.CreateDelegate(typeof(OfflineRaidAction), menuController, "method_62");
            gclass.ShowScreen(EScreenState.Queued);
        }
    }
}
