using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using HarmonyLib;
using EFT;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils;
using Aki.SinglePlayer.Utils.Reflection.CodeWrapper;

namespace Aki.SinglePlayer.Patches.ScavMode
{
    public class ExfilPointManagerPatch : GenericPatch<ExfilPointManagerPatch>
    {
        public ExfilPointManagerPatch() : base(transpiler: nameof(PatchTranspile))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.ExfilPointManagerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.CreateInstance).Single(IsTargetMethod);
        }

        static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var searchCode = new CodeInstruction(OpCodes.Call, AccessTools.Method(PatcherConstants.ExfilPointManagerType, "RemoveProfileIdFromPoints", new System.Type[] { typeof(string) }));
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
                PatchLogger.LogTranspileSearchError(MethodBase.GetCurrentMethod());
                return instructions;
            }

            searchIndex += 1;

            var newCodes = CodeGenerator.GenerateInstructions(new List<Code>()
            {
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, PatcherConstants.ExfilPointManagerType, "get_ScavExfiltrationPoints")
            });

            codes.RemoveRange(searchIndex, 23);
            codes.InsertRange(searchIndex, newCodes);

            return codes.AsEnumerable();
        }

        private static bool IsTargetMethod(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().Length == 3 && methodInfo.ReturnType == typeof(void);
        }
    }
}
