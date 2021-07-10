using System.Reflection;
using UnityEngine.Networking;
using Aki.Common.Utils.Patching;
using Aki.Core.Utils;

namespace Aki.Core.Patches
{
    public class FakeCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return ValidationUtil.Validate();
        }
    }

    public class UnityWebRequestPatch : GenericPatch<UnityWebRequestPatch>
    {
        private static CertificateHandler _certificateHandler = new FakeCertificateHandler();

        public UnityWebRequestPatch() : base(postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(UnityWebRequestTexture).GetMethod(nameof(UnityWebRequestTexture.GetTexture), new[] { typeof(string) });
        }

        private static void PatchPostfix(UnityWebRequest __result)
        {
            __result.certificateHandler = _certificateHandler;
            __result.disposeCertificateHandlerOnDispose = false;
            __result.timeout = 1000;
        }
    }
}
