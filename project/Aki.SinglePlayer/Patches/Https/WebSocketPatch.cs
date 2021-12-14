using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using System;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Https
{
    /// <summary>
    /// Goal: change the websocket system to use WS instead of WSS to ensure notifications show up in client and dont throw exceptions about missing certs
    /// </summary>
    public class WebSocketPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var targetInterface = PatchConstants.EftTypes.Single(x => x == typeof(IConnectionHandler) && x.IsInterface); // used to be GInterface138
            var typeThatMatches = PatchConstants.EftTypes.Single(x => targetInterface.IsAssignableFrom(x) && x.IsAbstract && !x.IsInterface); // Was Class1403
            return typeThatMatches.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(x => x.ReturnType == typeof(Uri)); // protected Uri method_0()
        }

        [PatchPostfix]
        private static Uri PatchPostfix(Uri __instance)
        {
            return new Uri(__instance.ToString().Replace("wss:", "ws:"));
        }
    }
}
