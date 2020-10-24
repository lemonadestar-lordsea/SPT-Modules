/* Target.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using NLog.Targets;
using SPTarkov.Common.Utils.Hook;

namespace SPTarkov.Common.Hook
{
	[Target("SPTarkov.Common")]
	public sealed class Target : TargetWithLayout
	{
		public Target()
		{
			Loader<Instance>.Load();
		}
	}
}
