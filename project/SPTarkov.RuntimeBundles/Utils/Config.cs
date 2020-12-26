/* Config.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using Comfort.Common;
using EFT;
using ISession = GInterface27;
using ClientConfig = GClass331;

namespace Aki.RuntimeBundles.Utils
{
    public static class Config
    {
        static Config()
        {
            _ = nameof(ISession.GetPhpSessionId);
            _ = nameof(ClientConfig.BackendUrl);
        }

        public static ISession BackEndSession => Singleton<ClientApplication>.Instance.GetClientBackEndSession();
        public static string BackendUrl => ClientConfig.Config.BackendUrl;
    }
}
