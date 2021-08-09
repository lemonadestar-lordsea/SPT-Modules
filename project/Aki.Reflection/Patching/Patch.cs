using System;
using System.Reflection;
using Aki.Common;
using HarmonyLib;

namespace Aki.Reflection.Patching
{
    public abstract class Patch
    {
        private Harmony _harmony;
        private HarmonyMethod _prefix;
        private HarmonyMethod _postfix;
        private HarmonyMethod _transpiler;
        private HarmonyMethod _finalizer;
        private HarmonyMethod _ilmanipulator;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="T">Type</param>
        /// <param name="name">Name</param>
        /// <param name="prefix">Prefix</param>
        /// <param name="postfix">Postfix</param>
        /// <param name="transpiler">Transpiler</param>
        /// <param name="finalizer">Finalizer</param>
        /// <param name="ilmanipulator">IL Manipulator</param>
        public Patch(Type T, string name = null, string prefix = null, string postfix = null, string transpiler = null, string finalizer = null, string ilmanipulator = null)
        {
            _harmony = new Harmony(name ?? T.Name);
            _prefix = GetPatchMethod(T, prefix);
            _postfix = GetPatchMethod(T, postfix);
            _transpiler = GetPatchMethod(T, transpiler);
            _finalizer = GetPatchMethod(T, finalizer);
            _ilmanipulator = GetPatchMethod(T, ilmanipulator);

            if (_prefix == null && _postfix == null && _transpiler == null && _finalizer == null && _ilmanipulator == null)
            {
                throw new Exception($"{_harmony.Id}: At least one of the patch methods must be specified");
            }
        }

        /// <summary>
        /// Get original method
        /// </summary>
        /// <returns>Method</returns>
        protected abstract MethodBase GetTargetMethod();

        /// <summary>
        /// Get HarmonyMethod from string
        /// </summary>
        /// <param name="T">Type</param>
        /// <param name="methodName">Method name</param>
        /// <returns>Method</returns>
        private HarmonyMethod GetPatchMethod(Type T, string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                return null;
            }

            return new HarmonyMethod(T.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
        }

        /// <summary>
        /// Apply patch to target
        /// </summary>
        public void Enable()
        {
            var targetMethod = GetTargetMethod();

            if (targetMethod == null)
            {
                throw new InvalidOperationException($"{_harmony.Id}: TargetMethod is null");
            }

            try
            {
                _harmony.Patch(targetMethod, _prefix, _postfix, _transpiler, _finalizer, _ilmanipulator);
                Log.Info($"Enabled patch {_harmony.Id}");
            }
            catch (Exception ex)
            {
                throw new Exception($"{_harmony.Id}:", ex);
            }
        }

        /// <summary>
        /// Remove applied patch from target
        /// </summary>
        public void Disable()
        {
            var targetMethod = GetTargetMethod();

            if (targetMethod == null)
            {
                throw new InvalidOperationException($"{_harmony.Id}: TargetMethod is null");
            }

            try
            {
                _harmony.Unpatch(targetMethod, HarmonyPatchType.All, _harmony.Id);
                Log.Info($"Disabled patch {_harmony.Id}");
            }
            catch (Exception ex)
            {
                throw new Exception($"{_harmony.Id}:", ex);
            }
        }
    }
}
