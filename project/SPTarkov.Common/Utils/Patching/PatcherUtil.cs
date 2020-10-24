/* PatcherUtil.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Martynas Gestautas
 * Merijn Hendriks
 */


using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace SPTarkov.Common.Utils.Patching
{
	public static class PatcherUtil
	{
		private static Harmony harmony;

		static PatcherUtil()
		{
			harmony = new Harmony("com.SPTarkov.common");
		}

		public static MethodInfo GetOriginalMethod<T>(string methodName)
		{
			return AccessTools.Method(typeof(T), methodName);
		}

        public static void Patch<T>() where T : GenericPatch<T>, new()
		{
			try
			{
				var patch = new T();
				if (patch.TargetMethod == null)
					throw new InvalidOperationException("TargetMethod is null");

				harmony.Patch(patch.TargetMethod,
							  prefix: patch.Prefix.ToHarmonyMethod(),
							  postfix: patch.Postfix.ToHarmonyMethod(),
							  transpiler: patch.Transpiler.ToHarmonyMethod(),
							  finalizer: patch.Finalizer.ToHarmonyMethod());
				Debug.LogError("SPTarkov.Common: Applied patch " + typeof(T).Name);
			}
			catch (Exception ex)
			{
				Debug.LogError($"SPTarkov.Common: Error in patch {typeof(T).Name}{Environment.NewLine}{ex}");
			}
		}
	}

	static class Extensions
	{
		public static HarmonyMethod ToHarmonyMethod(this MethodInfo methodInfo)
		{
			return methodInfo != null 
				? new HarmonyMethod(methodInfo)
				: null;
		}
	}
}
