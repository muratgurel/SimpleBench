using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Options;

namespace SimpleBench.Runner
{
	class MainClass
	{
		private static Type[] GetTypesWithAttribute(Type[] types, Type attributeType)
		{
			return types.Where(t =>
			{
				return t.GetCustomAttributes(attributeType, true).Length > 0;
			}).ToArray();
		}

		public static void Main(string[] args)
		{
			bool isVerbose = false;
			string dllPath = null;
			string outPath = Directory.GetCurrentDirectory();

			OptionSet optionSet = new OptionSet
			{
				{ "dll=", "The path of DLL containing benchmarks", path => dllPath = path },
				{ "o|out=", "The output path for reports", path => outPath = path },
				{ "v", "Verbose logging", v => isVerbose = (v != null)}
			};

			try
			{
				optionSet.Parse(args);

				if (string.IsNullOrEmpty(dllPath))
				{
					throw new ArgumentNullException(nameof(dllPath), "You should pass the benchmark dll with --dll=PATH option");
				}

				if (!dllPath.EndsWith(".dll", StringComparison.Ordinal))
				{
					dllPath += ".dll";
				}

				dllPath = Path.GetFullPath(dllPath);
				outPath = Path.GetFullPath(outPath);

				if (isVerbose)
				{
					Console.WriteLine("DLL Path: " + dllPath);
					Console.WriteLine("Out Path: " + outPath);	
				}

				Assembly benchmarkAssembly = Assembly.LoadFile(dllPath);

				var benchmarkFixtures = benchmarkAssembly.GetTypes()
				                                         .Where(t => t.GetCustomAttributes(typeof(BenchmarkFixtureAttribute), true).Length > 0)
				                                         .Select(t => Activator.CreateInstance(t));

				foreach (var fixture in benchmarkFixtures)
				{
					string fixtureName = fixture.GetType().Name;

					Console.WriteLine("\n------");
					Console.WriteLine(fixtureName);

					var benchmarkMethods = fixture.GetType()
					                              .GetMethods()
					                              .Where(t => t.GetCustomAttributes(typeof(BenchmarkAttribute), true).Length > 0);

					foreach (var method in benchmarkMethods)
					{
						var benchmark = new Benchmark();
						benchmark.N = 10;

						do
						{
							benchmark.Start();
							method.Invoke(fixture, new object[] { benchmark });
							benchmark.Stop();

							benchmark.N *= 10;
						}
						while (benchmark.elapsedMilliseconds < 1000);

						Time time = GetTime(benchmark.elapsedTicks / (double)benchmark.N);
						Console.WriteLine("{0}: {1} ops {2:F1} {3}/op", method.Name, benchmark.N, time.val, time.identifier);
					}
				}

				// For cosmetic reasons
				Console.WriteLine("");
			}
			catch (OptionException e)
			{
				Console.WriteLine(e.Message);
				return;
			}
			catch (ArgumentNullException e)
			{
				Console.WriteLine(e.Message);
				return;
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine(e.Message);
				return;
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
