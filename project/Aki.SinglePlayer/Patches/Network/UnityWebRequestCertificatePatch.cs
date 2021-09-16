using Aki.Reflection.Patching;
using System.Reflection;
using UnityEngine.Networking;

namespace Aki.SinglePlayer.Patches.Network
{
    public class UnityWebRequestCertificatePatch : Patch
    {
        private static readonly CertificateHandler _patchedCertificateHandler = new PatchedCertificateHandler();

        private class PatchedCertificateHandler : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData) => true;
        }

        public UnityWebRequestCertificatePatch() : base(typeof(UnityWebRequestCertificatePatch), postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(UnityWebRequest).GetMethod("InternalSetDefaults", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private static void PatchPostfix(UnityWebRequest __instance)
        {
            __instance.certificateHandler = _patchedCertificateHandler;
            __instance.disposeCertificateHandlerOnDispose = false;
        }
    }
}