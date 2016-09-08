using System;

namespace SimpleBench
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class BenchmarkAttribute : Attribute
	{
	}
}
