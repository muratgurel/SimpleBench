using System;

namespace SimpleBench
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class BenchmarkFixtureAttribute : Attribute
	{
	}
}
