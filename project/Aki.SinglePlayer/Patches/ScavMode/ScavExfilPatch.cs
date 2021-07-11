using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using EFT;
using Aki.Common;
using Aki.Reflection.CodeWrapper;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;

namespace Aki.SinglePlayer.Patches.ScavMode
{
    public class ScavExfilPatch : GenericPatch<ScavExfilPatch>
    {
        private static Type _profileType;
        private static Type _profileInfoType;
        private static Type _fenceTraderInfoType;

        public ScavExfilPatch() : base(transpiler: nameof(PatchTranspile))
        {
            _profileType = Constants.EftTypes.Single(x => x.GetMethod("AddToCarriedQuestItems") != null);
            _profileInfoType = Constants.EftTypes.Single(x => x.GetMethod("GetExperience") != null);
            _fenceTraderInfoType = Constants.EftTypes.Single(x => x.GetMethod("NewExfiltrationPrice") != null);
        }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.LocalGameType.BaseType
                    .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.CreateInstance)
                    .Single(IsTargetMethod);
        }

        private static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var searchCode = new CodeInstruction(OpCodes.Callvirt, Constants.ExfilPointManagerType
                .GetMethods().Single(x => x.Name == "EligiblePoints" && x.GetParameters()[0].Name == "entryPointName"));
            var searchIndex = -1;

            for (var i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == searchCode.opcode && codes[i].operand == searchCode.operand)
                {
                    searchIndex = i;
                    break;
                }
            }

            // Patch failed.
            if (searchIndex == -1)
            {
                Log.Error(string.Format("Patch {0} failed: Could not find reference code.", MethodBase.GetCurrentMethod()));
                return instructions;
            }

            searchIndex -= 3;

            var brFalseLabel = generator.DefineLabel();
            var brLabel = generator.DefineLabel();
            var newCodes = CodeGenerator.GenerateInstructions(new List<Code>()
            {
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Info"),
                new Code(OpCodes.Ldfld, _profileInfoType, "Side"),
                new Code(OpCodes.Ldc_I4_4),
                new Code(OpCodes.Ceq),
                new Code(OpCodes.Brfalse, brFalseLabel),
                new Code(OpCodes.Call, Constants.ExfilPointManagerType, "get_Instance"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Ldfld, Constants.LocalGameType.BaseType, "gparam_0"),
                new Code(OpCodes.Box, typeof(PlayerOwner)),
                new Code(OpCodes.Callvirt, typeof(PlayerOwner), "get_Player"),
                new Code(OpCodes.Callvirt, typeof(Player), "get_Position"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Id"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Call, _profileType, "get_FenceInfo"),
                new Code(OpCodes.Call, _fenceTraderInfoType, "get_AvailableExitsCount"),
                new Code(OpCodes.Callvirt, Constants.ExfilPointManagerType, "ScavExfiltrationClaim", new Type[]{ typeof(Vector3), typeof(string), typeof(int) }),
                new Code(OpCodes.Call, Constants.ExfilPointManagerType, "get_Instance"),
                new Code(OpCodes.Call, Constants.ExfilPointManagerType, "get_Instance"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Id"),
                new Code(OpCodes.Callvirt, Constants.ExfilPointManagerType, "GetScavExfiltrationMask"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Ldfld, typeof(Profile), "Id"),
                new Code(OpCodes.Callvirt, Constants.ExfilPointManagerType, "ScavExfiltrationClaim", new Type[]{ typeof(int), typeof(string) }),
                new Code(OpCodes.Br, brLabel),
                new CodeWithLabel(OpCodes.Call, brFalseLabel, Constants.ExfilPointManagerType, "get_Instance"),
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.LocalGameType.BaseType, "get_Profile_0"),
                new Code(OpCodes.Callvirt, Constants.ExfilPointManagerType, "EligiblePoints", new Type[]{ typeof(Profile) }),
                new CodeWithLabel(OpCodes.Stloc_2, brLabel)
            });

            codes.RemoveRange(searchIndex, 5);
            codes.InsertRange(searchIndex, newCodes);

            return codes.AsEnumerable();
        }

        private static bool IsTargetMethod(MethodInfo methodInfo)
        {
            return methodInfo.IsVirtual
                && methodInfo.GetParameters().Length == 0
                && methodInfo.ReturnType == typeof(void)
                && methodInfo.GetMethodBody().LocalVariables.Count > 0;
        }
    }
}
