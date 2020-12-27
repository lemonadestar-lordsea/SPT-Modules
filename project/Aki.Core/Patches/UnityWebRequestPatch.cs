/* UnityWebRequestPatch.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using System.Reflection;
using UnityEngine.Networking;
using Aki.Common.Utils.Patching;

namespace Aki.Core.Patches
{
    class UnityWebRequestPatch : GenericPatch<UnityWebRequestPatch>
    {
        private static readonly CertificateHandler _certificateHandler = new FakeCertificateHandler();

        public UnityWebRequestPatch() : base(postfix: nameof(PatchPostfix)) {}

        protected override MethodBase GetTargetMethod()
        {
            return typeof(UnityWebRequestTexture).GetMethod(nameof(UnityWebRequestTexture.GetTexture), new[] { typeof(string) });
        }

        static void PatchPostfix(UnityWebRequest __result)
        {
            __result.certificateHandler = _certificateHandler;
            __result.disposeCertificateHandlerOnDispose = false;
            __result.timeout = 1000;
        }

        internal class FakeCertificateHandler : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }
    }
}
