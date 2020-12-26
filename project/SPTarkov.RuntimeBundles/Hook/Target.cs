/* Target.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 */


using NLog.Targets;
using Aki.Common.Utils.Hook;
using Aki.RuntimeBundles;

namespace Aki.RuntimeBundles.Hook
{
	[Target("Aki.RuntimeBundles")]
	public sealed class Target : TargetWithLayout
	{
		public Target()
		{
			Loader<Instance>.Load();
		}
	}
}
