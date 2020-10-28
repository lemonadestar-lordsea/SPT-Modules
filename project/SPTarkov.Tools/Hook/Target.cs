/* Target.cs
 * License: NCSA Open Source License
 * 
 * Copyright: SPT AKI
 * AUTHORS:
 * Ginja
 */


using NLog.Targets;
using SPTarkov.Common.Utils.Hook;

namespace SPTarkov.Tools.Hook
{
	[Target("SPTarkov.Tools")]
	public sealed class Target : TargetWithLayout
	{
		public Target()
		{
			Loader<Instance>.Load();
		}
	}
}
