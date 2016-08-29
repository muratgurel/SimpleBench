using System;

namespace SimpleBench
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class IterationsAttribute : Attribute
	{
		public readonly int value;

		public IterationsAttribute(int value)
		{
			this.value = value;
		}
	}
}
