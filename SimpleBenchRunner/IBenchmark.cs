using System;

namespace SimpleBench
{
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
		/// Call this before starting any benchmarks!
		/// </summary>
		void SetUp();

		/// <summary>
		/// Do the benchmark.
		/// </summary>
		void DoBench();

		/// <summary>
		/// You can run inner benchmarks inside your main benchmark function.
		/// </summary>
		/// <param name="name">Name of inner benchmark for reports</param>
		/// <param name="innerBenchmark">Inner benchmark function</param>
		void Run(string name, Action<IBenchmark> innerBenchmark);
	}
}
