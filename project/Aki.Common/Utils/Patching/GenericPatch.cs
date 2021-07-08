using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace Aki.Common.Utils.Patching
{
    public abstract class GenericPatch<T> where T : GenericPatch<T>
    {
        private Harmony _harmony;
        private MethodBase _targetMethod;
        private PatchMethod _prefix;
        private PatchMethod _postfix;
        private PatchMethod _transpiler;
        private PatchMethod _finalizer;

        public GenericPatch(string name = null, string prefix = null, string postfix = null, string transpiler = null, string finalizer = null)
        {
            _harmony = new Harmony(name ?? typeof(T).Name);
            _targetMethod = GetTargetMethod();
            _prefix = GetPatchMethod(prefix);
            _postfix = GetPatchMethod(postfix);
            _transpiler = GetPatchMethod(transpiler);
            _finalizer = GetPatchMethod(finalizer);

            if (_targetMethod == null)
            {
                throw new InvalidOperationException("TargetMethod is null");
            }

            if (_prefix == null && _postfix == null && _transpiler == null && _finalizer == null)
            {
                throw new Exception("At least one of the patch methods must be specified");
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
        private PatchMethod GetPatchMethod(string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                return null;
            }

            return typeof(T).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
        }

        /// <summary>
        /// Apply patch to target
        /// </summary>
        public void Apply()
        {
            try
            {
                _harmony.Patch(_targetMethod, _prefix, _postfix, _transpiler, _finalizer);
                Debug.LogError("Aki.Common: Applied patch " + typeof(T).Name);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Aki.Common: Error in applying patch {typeof(T).Name}{Environment.NewLine}{ex}");
            }
        }

        /// <summary>
        /// Remove applied patch from target
        /// </summary>
        public void Remove()
        {
            try
            {
                _harmony.Unpatch(_targetMethod, HarmonyPatchType.All, _harmony.Id);
                Debug.LogError("Aki.Common: Removed patch " + typeof(T).Name);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Aki.Common: Error in removing patch {typeof(T).Name}{Environment.NewLine}{ex}");
            }
        }
    }
}
