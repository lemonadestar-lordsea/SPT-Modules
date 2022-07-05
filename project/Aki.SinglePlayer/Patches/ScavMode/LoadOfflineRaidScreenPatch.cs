﻿using Aki.Reflection.Patching;
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

            _onReadyScreenMethod = menuControllerType.GetMethod("method_37", PatchConstants.PrivateFlags);
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
            return typeof(MainMenuController).GetMethod("method_58", PatchConstants.PrivateFlags);
        }

        [PatchTranspiler]
        private static IEnumerable<CodeInstruction> PatchTranspiler(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // The original method call that we want to replace
            var onReadyScreenMethodIndex = -1;
            var onReadyScreenMethodCode = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MainMenuController), _onReadyScreenMethod.Name));

            // We additionally need to replace an instruction that jumps to a label on certain conditions, since we change the jump target instruction
            var jumpWhenFalse_Index = -1;

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == onReadyScreenMethodCode.opcode && codes[i].operand == onReadyScreenMethodCode.operand)
                {
                    onReadyScreenMethodIndex = i;
                    continue;
                }

                if (codes[i].opcode == OpCodes.Brfalse_S)
                {
                    if (jumpWhenFalse_Index != -1)
                    {
                        // If this warning is ever logged, the condition for locating the exact brfalse.s instruction will have to be updated
                        Logger.LogWarning($"[{nameof(LoadOfflineRaidScreenPatch)}] Found extra instructions with the brfalse.s opcode! " +
                                          "This breaks an old assumption that there is only one such instruction in the method body and is now very likely to cause bugs!");
                    }
                    jumpWhenFalse_Index = i;
                }
            }

            if (onReadyScreenMethodIndex == -1)
            {
                throw new Exception($"{nameof(LoadOfflineRaidScreenPatch)} failed: Could not find {nameof(_onReadyScreenMethod)} reference code.");
            }

            if (jumpWhenFalse_Index == -1)
            {
                throw new Exception($"{nameof(LoadOfflineRaidScreenPatch)} failed: Could not find jump (brfalse.s) reference code.");
            }

            // Define the new jump label
            var brFalseLabel = generator.DefineLabel();

            // We build the method call for our substituted method and replace the initial method call with our own, also adding our new label
            var callCode = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LoadOfflineRaidScreenPatch), nameof(LoadOfflineRaidScreenForScav))) { labels = { brFalseLabel }};
            codes[onReadyScreenMethodIndex] = callCode;

            // We build a new brfalse.s instruction and give it our new label, then replace the original brfalse.s instruction
            var newBrFalseCode = new CodeInstruction(OpCodes.Brfalse_S, brFalseLabel);
            codes[jumpWhenFalse_Index] = newBrFalseCode;

            // This will remove a stray ldarg.0 instruction. It's only needed if we wanted to reference something from `this` in the method body.
            // This is done last to ensure that previous instruction indexes don't shift around (probably why this used to just turn it into a Nop OpCode)
            codes.RemoveAt(onReadyScreenMethodIndex - 1);

            return codes.AsEnumerable();
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

        private static MainMenuController GetMenuController()
        {
            return _menuControllerField.GetValue(ClientAppUtils.GetMainApp()) as MainMenuController;
        }
    }
}