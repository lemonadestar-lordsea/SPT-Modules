using System.Linq;
using System.Reflection;
using UnityEngine.Networking;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.Core.Utils;

namespace Aki.Core.Patches
{
	public class SslCertificatePatch : GenericPatch<SslCertificatePatch>
	{
		public SslCertificatePatch() : base(prefix: nameof(PatchPrefix))
		{
		}

		protected override MethodBase GetTargetMethod()
		{
			return Constants.EftTypes.Single(x => x.BaseType == typeof(CertificateHandler))
				.GetMethod("ValidateCertificate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		private static bool PatchPrefix(ref bool __result)
		{
			__result = ValidationUtil.Validate();
			return false;
		}
	}
}
