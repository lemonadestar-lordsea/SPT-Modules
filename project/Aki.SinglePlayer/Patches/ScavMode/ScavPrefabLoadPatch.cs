using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using EFT;
using Aki.Common;
using Aki.Reflection.CodeWrapper;
using Aki.Reflection.Utils;
using Patch = Aki.Reflection.Patching.Patch;

namespace Aki.SinglePlayer.Patches.ScavMode
{
    public class ScavPrefabLoadPatch : Patch
    {
        public ScavPrefabLoadPatch() : base(T: typeof(ScavPrefabLoadPatch), transpiler: nameof(PatchTranspile))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainApplication)
                .GetNestedTypes(Constants.PrivateFlags)
                .Single(x => 
                    x.GetField("entryPoint") != null 
                    && x.GetField("timeAndWeather") != null
                    && x.GetField("location") != null
                    && x.GetField("mainApplication_0") != null
                    && x.GetField("timeHasComeScreenController") == null
                    && x.Name.Contains("Struct"))
                .GetMethods(Constants.PrivateFlags)
                .FirstOrDefault(x => x.Name == "MoveNext");
        }

        private static IEnumerable<CodeInstruction> PatchTranspile(ILGenerator generator, IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            // Search for code where backend.Session.getProfile() is called.
            var searchCode = new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Constants.SessionInterfaceType, "get_Profile"));
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

            // Move back by 3. This is the start of IL chain that we're interested in.
            searchIndex -= 3;

            var brFalseLabel = generator.DefineLabel();
            var brLabel = generator.DefineLabel();

            var newCodes = CodeGenerator.GenerateInstructions(new List<Code>()
            {
                new Code(OpCodes.Ldloc_1),
                new Code(OpCodes.Ldfld, typeof(ClientApplication), "_backEnd"),
                new Code(OpCodes.Callvirt, Constants.BackendInterfaceType, "get_Session"),
                new Code(OpCodes.Ldloc_1),
                new Code(OpCodes.Ldfld, typeof(MainApplication), "esideType_0"),
                new Code(OpCodes.Ldc_I4_0),
                new Code(OpCodes.Ceq),
                new Code(OpCodes.Brfalse, brFalseLabel),
                new Code(OpCodes.Callvirt, Constants.SessionInterfaceType, "get_Profile"),
                new Code(OpCodes.Br, brLabel),
                new CodeWithLabel(OpCodes.Callvirt, brFalseLabel, Constants.SessionInterfaceType, "get_ProfileOfPet"),
                new CodeWithLabel(OpCodes.Ldc_I4_1, brLabel)
            });

            codes.RemoveRange(searchIndex, 5);
            codes.InsertRange(searchIndex, newCodes);
            return codes.AsEnumerable();
        }
    }
}
