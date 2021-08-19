using System;
using System.Runtime.InteropServices;

namespace Aki.Loader
{
    public static class WinApi
    {
        // https://docs.microsoft.com/en-us/dotnet/framework/unmanaged-api/strong-naming/strongnamesignatureverificationex-function
        [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
        private static extern bool StrongNameSignatureVerificationEx(string wszFilePath, byte fForceVerification, ref byte pfWasVerified);

        // using byte for arguments rather than bool, because bool won't work on 64-bit Windows
        public static bool CheckSignature(string assemblyPath)
        {
            byte wasVerified = Convert.ToByte(false);
            bool result = StrongNameSignatureVerificationEx(assemblyPath, Convert.ToByte(true), ref wasVerified);
            return wasVerified == Convert.ToByte(true) && result == true;
        }
    }
}
