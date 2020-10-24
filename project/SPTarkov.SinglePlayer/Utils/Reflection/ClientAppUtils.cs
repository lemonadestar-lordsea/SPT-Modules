/* ClientAppUtils.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using UnityEngine;
using Comfort.Common;
using EFT;

namespace SPTarkov.SinglePlayer.Utils.Reflection
{
    internal static class ClientAppUtils
    {
        public static ClientApplication GetClientApp()
        {
            return Singleton<ClientApplication>.Instance;
        }

        public static MainApplication GetMainApp()
        {
            return GetClientApp() as MainApplication;
        }
    }
}
