using Comfort.Common;
using EFT;
using ISession = GInterface106;
using ClientConfig = GClass350;

namespace Aki.SinglePlayer.Utils
{
    public static class Config
    {
        static Config()
        {
            _ = nameof(ISession.GetPhpSessionId);
            _ = nameof(ClientConfig.BackendUrl);
        }

        private static ISession _backEndSession;
        public static ISession BackEndSession
        {
            get
            {
                if (_backEndSession == null)
                {
                    _backEndSession = Singleton<ClientApplication>.Instance.GetClientBackEndSession();
                }

                return _backEndSession;
            }
        }

        private static string _backendUrl;
        public static string BackendUrl
        {
            get
            {
                if (BackEndSession != null && _backendUrl == null)
                {
                    _backendUrl = ClientConfig.Config.BackendUrl;
                }

                return _backendUrl;
            }
        }
    }
}
