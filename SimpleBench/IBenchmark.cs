using System;

namespace SimpleBench
{
	/// <summary>
	/// This interface is used in benchmark methods marked with
	/// [Benchmark] attribute. The runner passes a benchmark object
	/// as the parameter of the benchmark method, so the user can
	/// Reset benchmark timer or run sub-benchmarks with Run method.
	/// </summary>
	public interface IBenchmark
	{
		/// <summary>
		/// Number of times you should run your benchmark function. This number
		/// is adjusted automatically.
		/// </summary>
		/// <value>The n.</value>
		int N
		{
			get;
		}

		/// <summary>
		/// Resets timer. Use this if you are doing something like initialization
		/// inside the benchmark function. Do all those things and then reset the
		/// timer jsut before you run the actual function that you will benchmark.
		/// </summary>
		void Reset();

		/// <summary>
		/// You can run inner benchmarks inside your main benchmark function.
		/// </summary>
		/// <param name="name">Name of inner benchmark for reports</param>
		/// <param name="innerBenchmark">Inner benchmark function</param>
		void Run(string name, Action<IBenchmark> innerBenchmark);
	}
}
