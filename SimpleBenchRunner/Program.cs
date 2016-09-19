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
						var benchmark = new Benchmark(method.Name, fixture, method);
						benchmark.N = 10;

						benchmark.SetUp();
						benchmark.DoBench(isVerbose);
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
				// When DLL cannot be opened
				Console.WriteLine(e.Message);
				return;
			}
		}
	}
}
