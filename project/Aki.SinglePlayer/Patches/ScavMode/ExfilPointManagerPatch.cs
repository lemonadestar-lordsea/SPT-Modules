using Aki.Common;
using Aki.Reflection.CodeWrapper;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Aki.SinglePlayer.Patches.ScavMode
{
    public class ExfilPointManagerPatch : ModulePatch
    {
        public ExfilPointManagerPatch() : base(T: typeof(ExfilPointManagerPatch), transpiler: nameof(PatchTranspile))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.ExfilPointManagerType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.CreateInstance)
                .Single(IsTargetMethod);
        }

        private static bool IsTargetMethod(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().Length == 3 && methodInfo.ReturnType == typeof(void);
        }

        private static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var searchCode = new CodeInstruction(OpCodes.Call, AccessTools.Method(Constants.ExfilPointManagerType, "RemoveProfileIdFromPoints"));
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

            searchIndex += 1;

            var newCodes = CodeGenerator.GenerateInstructions(new List<Code>()
            {
                new Code(OpCodes.Ldarg_0),
                new Code(OpCodes.Call, Constants.ExfilPointManagerType, "get_ScavExfiltrationPoints")
            });

            codes.RemoveRange(searchIndex, 23);
            codes.InsertRange(searchIndex, newCodes);
            return codes.AsEnumerable();
        }
    }
}
