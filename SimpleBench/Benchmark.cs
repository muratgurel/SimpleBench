using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleBench
{
	public class Benchmark : IBenchmark
	{
		public int N
		{
			get;
			set;
		}

		public string name
		{
			get;
			private set;
		}

		public long elapsedMilliseconds
		{
			get
			{
				return stopwatch.ElapsedMilliseconds;
			}
		}

		public long elapsedTicks
		{
			get
			{
				return stopwatch.ElapsedTicks;
			}
		}

		public List<Benchmark> innerBenchmarks
		{
			get;
			private set;
		}

		private readonly Stopwatch stopwatch;

		// For doing fixture level benchmarks
		private readonly object fixture;
		private readonly MethodInfo method;

		// For doing inner benchmarks
		private readonly Action<IBenchmark> benchmarkFunction;

		public Benchmark(string name, object fixture, MethodInfo method)
		{
			this.name = name;
			this.method = method;
			this.fixture = fixture;
			this.stopwatch = new Stopwatch();
			this.innerBenchmarks = new List<Benchmark>();
		}

		private Benchmark(string name, Action<IBenchmark> benchmarkFunction)
		{
			this.name = name;
			this.stopwatch = new Stopwatch();
			this.benchmarkFunction = benchmarkFunction;
			this.innerBenchmarks = new List<Benchmark>();
		}

		public void Reset()
		{
			stopwatch.Restart();
		}

		public void Run(string name, Action<IBenchmark> innerBenchmark)
		{
			innerBenchmarks.Add(new Benchmark(this.name + name, innerBenchmark));
		}

		public void Start()
		{
			Reset();
		}

		public void Stop()
		{
			stopwatch.Stop();
		}

		public void SetUp()
		{
			if (method != null)
			{
				// Invoke the method once to collect any inner benchmarks it may have.
				method.Invoke(fixture, new object[] { this });
			}
		}

		public void DoBench()
		{
			if (innerBenchmarks.Count > 0)
			{
				// Bench inner ones
				foreach (var innerBench in innerBenchmarks)
				{
					innerBench.N = 10;

					innerBench.SetUp();
					innerBench.DoBench();
				}
			}
			else
			{
				if (method != null)
				{
					// Top level bench
					// TODO: Add a fail case, like a max N
					while (true)
					{
						Start();
						method.Invoke(fixture, new object[] { this });
						Stop();

						if (elapsedMilliseconds >= 1000)
						{
							// Test finished
							break;
						}

						N *= 10;
					}
				}

				if (benchmarkFunction != null)
				{
					// Inner bench
					while (true)
					{
						Start();
						benchmarkFunction(this);
						Stop();

						if (elapsedMilliseconds >= 1000)
						{
							// Test finished
							break;
						}

						N *= 10;
					}
				}

				Time time = GetTime(elapsedTicks / (double)N);
				Console.WriteLine("{0}: {1} ops {2:F1} {3}/op", name, N, time.val, time.identifier);
			}
		}

		private static Time GetTime(double ticks)
		{
			if (ticks < 10d)
			{
				// Nanoseconds
				return new Time(ticks / 0.01d, "ns");
			}

			if (ticks < 10000d)
			{
				// Microseconds
				return new Time(ticks / 10d, "μs");
			}

			if (ticks < 10000000d)
			{
				// Milliseconds
				return new Time(ticks / 10000d, "ms");
			}

			// Seconds
			return new Time(ticks / 10000000d, "s");
		}

		private struct Time
		{
			public double val;
			public string identifier;

			public Time(double val, string identifier)
			{
				this.val = val;
				this.identifier = identifier;
			}
		}
	}
}
