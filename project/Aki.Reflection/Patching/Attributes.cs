using System;

namespace Aki.Reflection.Patching
{
	[AttributeUsage(AttributeTargets.Method)]
	public class PatchPrefixAttribute : Attribute
    {
    }

	[AttributeUsage(AttributeTargets.Method)]
	public class PatchPostfixAttribute : Attribute
    {
    }

	[AttributeUsage(AttributeTargets.Method)]
	public class PatchTranspilerAttribute : Attribute
    {
    }

	[AttributeUsage(AttributeTargets.Method)]
	public class PatchFinalizerAttribute : Attribute
    {
    }

	[AttributeUsage(AttributeTargets.Method)]
	public class PatchILManipulatorAttribute : Attribute
    {
    }
}
