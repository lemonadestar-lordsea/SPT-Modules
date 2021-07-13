using System;
using System.Reflection;
using HarmonyLib;
using Aki.Common;

namespace Aki.Reflection.Patching
{
    public abstract class GenericPatch<T> where T : GenericPatch<T>
    {
        private Harmony _harmony;
        private HarmonyMethod _prefix;
        private HarmonyMethod _postfix;
        private HarmonyMethod _transpiler;
        private HarmonyMethod _finalizer;
        private HarmonyMethod _ilmanipulator;

        public GenericPatch(string name = null, string prefix = null, string postfix = null, string transpiler = null, string finalizer = null, string ilmanipulator = null)
        {
            _harmony = new Harmony(name ?? typeof(T).Name);
            _prefix = GetPatchMethod(prefix);
            _postfix = GetPatchMethod(postfix);
            _transpiler = GetPatchMethod(transpiler);
            _finalizer = GetPatchMethod(finalizer);
            _ilmanipulator = GetPatchMethod(ilmanipulator);

            if (_prefix == null && _postfix == null && _transpiler == null && _finalizer == null && _ilmanipulator == null)
            {
                Log.Error($"{_harmony.Id}: At least one of the patch methods must be specified");
            }
        }

        /// <summary>
        /// Get original method
        /// </summary>
        /// <returns>Method</returns>
        protected abstract MethodBase GetTargetMethod();

        /// <summary>
        /// Get MethodInfo from string
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <returns>Method</returns>
        private HarmonyMethod GetPatchMethod(string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                return null;
            }

            return new HarmonyMethod(typeof(T).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
        }

        /// <summary>
        /// Apply patch to target
        /// </summary>
        public void Apply()
        {
            var targetMethod = GetTargetMethod();

            if (targetMethod == null)
            {
                Log.Error($"{_harmony.Id}: TargetMethod is null");
            }

            try
            {
                _harmony.Patch(targetMethod, _prefix, _postfix, _transpiler, _finalizer, _ilmanipulator);
                Log.Info($"{_harmony.Id}: Applied successfully");
            }
            catch (Exception ex)
            {
                Log.Error($"{_harmony.Id} Failed to apply");
                Log.Write(ex.Message);
                Log.Write(ex.StackTrace);
            }
        }

        /// <summary>
        /// Remove applied patch from target
        /// </summary>
        public void Remove()
        {
            var targetMethod = GetTargetMethod();

            if (targetMethod == null)
            {
                Log.Error($"{_harmony.Id}: TargetMethod is null");
            }

            try
            {
                _harmony.Unpatch(targetMethod, HarmonyPatchType.All, _harmony.Id);
                Log.Info($"{_harmony.Id}: Removed successfully");
            }
            catch (Exception ex)
            {
                Log.Error($"{_harmony.Id}: Failed to remove");
                Log.Write(ex.Message);
                Log.Write(ex.StackTrace);
            }
        }
    }
}
