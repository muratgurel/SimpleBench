using System.Collections.Generic;

namespace SimpleBench.Runner
{
	public struct BenchmarkResult
	{
		public int runCount
		{
			get
			{
				return measures.Count;
			}
		}

		public readonly string name;
		public readonly List<double> measures;

		public BenchmarkResult(string name, List<double> measures)
		{
			this.name = name;
			this.measures = measures;
		}
	}
}
