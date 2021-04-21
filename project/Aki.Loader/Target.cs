using NLog.Targets;
using UnityEngine;

namespace Aki.Loader
{
	[Target("Aki.Loader")]
	public sealed class Target : TargetWithLayout
	{
		public static GameObject HookObject
		{
			get
			{
				GameObject result = GameObject.Find("Aki.Hook");

				if (result == null)
				{
					result = new GameObject("Aki.Hook");
					Object.DontDestroyOnLoad(result);
				}

				return result;
			}
		}

		public Target()
		{
			HookObject.GetOrAddComponent<Instance>();
		}
	}
}
