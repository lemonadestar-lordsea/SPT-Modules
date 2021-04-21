using Comfort.Common;
using EFT;
using ISession = GInterface28;
using ClientConfig = GClass334;

namespace Aki.SinglePlayer.Utils
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
        public static bool WeaponDurabilityEnabled = false;
    }
}
