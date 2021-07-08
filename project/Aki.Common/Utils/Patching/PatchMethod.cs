using System.Reflection;
using HarmonyLib;

namespace Aki.Common.Utils.Patching
{
    public class PatchMethod
    {
        private MethodInfo _methodInfo;
        private HarmonyMethod _harmonyMethod;

        public PatchMethod(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            _harmonyMethod = new HarmonyMethod(methodInfo);
        }

        public PatchMethod(HarmonyMethod harmonyMethod)
        {
            _methodInfo = harmonyMethod.method;
            _harmonyMethod = harmonyMethod;
        }

        /// <summary>
        /// PatchMethod to HarmonyMethod
        /// </summary>
        /// <param name="self">Instance</param>
        public static implicit operator HarmonyMethod(PatchMethod self)
        {
            return self._harmonyMethod;
        }

        /// <summary>
        /// MethodInfo to HarmonyMethod
        /// </summary>
        /// <param name="self">Instance</param>
        public static implicit operator MethodInfo(PatchMethod self)
        {
            return self._methodInfo;
        }

        /// <summary>
        /// HarmonyMethod to PatchMethod
        /// </summary>
        /// <param name="self">Instance</param>
        public static implicit operator PatchMethod(HarmonyMethod self)
        {
            return new PatchMethod(self);
        }

        /// <summary>
        /// MethodInfo to PatchMethod
        /// </summary>
        /// <param name="self">Instance</param>
        public static implicit operator PatchMethod(MethodInfo self)
        {
            return new PatchMethod(self);
        }
    }
}
