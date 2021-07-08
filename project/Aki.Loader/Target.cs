using NLog.Targets;

namespace Aki.Loader
{
	[Target("Aki.Loader")]
	public sealed class Target : TargetWithLayout
	{
		public Target()
		{
			Program.Main(null);
		}
	}
}
