using System;
using System.Diagnostics;

namespace SimpleBench
{
	public class Benchmark : IBenchmark
	{
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

		public int N
		{
			get;
			set;
		}

		private readonly Stopwatch stopwatch;

		public Benchmark()
		{
			stopwatch = new Stopwatch();
		}

		public void Reset()
		{
			stopwatch.Restart();
		}

		public void Run(Action<IBenchmark> innerBenchmark)
		{
			throw new NotImplementedException();
		}

		public void Start()
		{
			Reset();
		}

		public void Stop()
		{
			stopwatch.Stop();
		}
	}
}
