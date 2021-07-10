using Comfort.Common;
using EFT;
using ISession = GInterface106;

namespace Aki.SinglePlayer.Utils
{
    public static class Config
    {
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

        static Config()
        {
            _ = nameof(ISession.GetPhpSessionId);
        }
    }
}
