using System;
using System.Linq;
using Comfort.Common;
using EFT;
using ISession = GInterface106;

namespace Aki.Reflection.Utils
{
    public static class Constants
    {
        public static Type[] EftTypes { get; private set; }
        public static Type LocalGameType { get; private set; }
        public static Type ExfilPointManagerType { get; private set; }
        public static Type BackendInterfaceType { get; private set; }
        public static Type SessionInterfaceType { get; private set; }
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

        static Constants()
        {
            _ = nameof(ISession.GetPhpSessionId);

            EftTypes = typeof(AbstractGame).Assembly.GetTypes();
            LocalGameType = EftTypes.Single(x => x.Name == "LocalGame");
            ExfilPointManagerType = EftTypes.Single(x => x.GetMethod("InitAllExfiltrationPoints") != null);
            BackendInterfaceType = EftTypes.Single(x => x.GetMethods().Select(y => y.Name).Contains("CreateClientSession") && x.IsInterface);
            SessionInterfaceType = EftTypes.Single(x => x.GetMethods().Select(y => y.Name).Contains("GetPhpSessionId") && x.IsInterface);
        }
    }
}
