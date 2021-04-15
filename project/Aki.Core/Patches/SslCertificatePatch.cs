using System.Linq;
using System.Reflection;
using UnityEngine.Networking;
using Aki.Common.Utils.Patching;
using Aki.Core.Utils;

namespace Aki.Core.Patches
{
	public class SslCertificatePatch : GenericPatch<SslCertificatePatch>
	{
		public SslCertificatePatch() : base(prefix: nameof(PatchPrefix)) {}

		protected override MethodBase GetTargetMethod()
		{
			return PatcherConstants.TargetAssembly
				.GetTypes().Single(x => x.BaseType == typeof(CertificateHandler))
				.GetMethod("ValidateCertificate", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		}

		static bool PatchPrefix(ref bool __result)
		{
			__result = ValidationUtil.Validate();
			return false;
		}
	}
}
