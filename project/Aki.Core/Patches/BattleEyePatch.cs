/* BattleEyePatch.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Martynas Gestautas
 * Merijn Hendriks
 */


using Microsoft.Win32;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aki.Common.Utils.Patching;

namespace Aki.Core.Patches
{
	public class BattleEyePatch : GenericPatch<BattleEyePatch>
	{
        public static PropertyInfo __property;
		
        public BattleEyePatch() : base(prefix: nameof(PatchPrefix)) {}

		protected override MethodBase GetTargetMethod()
		{
			var methodName = "RunValidation";
			var flags = BindingFlags.Public | BindingFlags.Instance;
			var __type = PatcherConstants.TargetAssembly.GetTypes().Single(x => x.GetMethod(methodName, flags) != null);

            __property = __type.GetProperty("Succeed", flags);

            return __type.GetMethod(methodName, flags);
        }

        private static bool PatchPrefix(ref Task __result, object __instance)
		{
            __property.SetValue(__instance, Validate());
			__result = Task.CompletedTask;

			return false;
		}

		private static bool Validate()
        {
			var c0 = @"Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\EscapeFromTarkov";
			var v0 = 0;

            try
            {
                var v1 = Registry.LocalMachine.OpenSubKey(c0, false).GetValue("UninstallString");
                var v2 = (v1 != null) ? v1.ToString() : "";
                var v3 = new FileInfo(v2);
                var v4 = new FileInfo[]
                {
                    v3,
                    new FileInfo(v2.Replace(v3.Name, @"BattlEye\BEClient_x64.dll")),
                    new FileInfo(v2.Replace(v3.Name, @"BattlEye\BEService_x64.dll")),
					new FileInfo(v2.Replace(v3.Name, @"ConsistencyInfo")),
					new FileInfo(v2.Replace(v3.Name, @"Uninstall.exe")),
					new FileInfo(v2.Replace(v3.Name, @"UnityCrashHandler64.exe")),
					new FileInfo(v2.Replace(v3.Name, @"WinPixEventRuntime.dll"))
                };

                v0 = v4.Length - 1;

                foreach (var value in v4)
                {
                    if (File.Exists(value.FullName))
                    {
                        --v0;
                    }
                }
            }
            catch
            {
                v0 = -1;
            }

            return v0 == 0;
        }
	}
}
