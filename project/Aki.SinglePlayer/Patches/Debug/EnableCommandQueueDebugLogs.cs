using System.Linq;
using System.Reflection;
using Aki.Common.Utils.Patching;

namespace Aki.Singleplayer.Patches.Debug
{
    public class EnableCommandQueueDebugLogs : GenericPatch<EnableCommandQueueDebugLogs>
    {
        public EnableCommandQueueDebugLogs(): base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            GClass389.IsLogsEnabled = true;
            return typeof(GClass389).GetMethods().Single(m => m.Name == "Log" && m.GetParameters().Length == 3 && m.GetParameters()[0].Name == "logLevel" && m.GetParameters()[1].Name == "format" && m.IsVirtual);
        }

        public static void PatchPrefix(string format, object[] args)
        {
            UnityEngine.Debug.LogError(string.Format(format, args));
        }
    }
}