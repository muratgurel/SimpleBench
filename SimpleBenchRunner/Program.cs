﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Options;

namespace SimpleBench.Runner
{
	class MainClass
	{
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

				var benchmarks = benchmarkAssembly.GetTypes()
				                                  .Where(t => t.GetInterfaces().Contains(typeof(IBenchmark)) &&
														t.GetConstructor(Type.EmptyTypes) != null)
				                                  .Select(t => Activator.CreateInstance(t) as IBenchmark);

				var stopWatch = new Stopwatch();
				var benchmarkResults = new Dictionary<string, List<Double>>();

				foreach (var benchmark in benchmarks)
				{
					string benchmarkName = benchmark.GetType().ToString();

					Console.WriteLine("\n------");
					Console.WriteLine(benchmarkName);

					benchmark.SetUp();

					int iterations = 1;

					MethodBase runMethod = benchmark.GetType().GetMethod("Run");
					IterationsAttribute attr = runMethod.GetCustomAttributes(typeof(IterationsAttribute), true).FirstOrDefault() as IterationsAttribute;

					if (attr != null)
					{
						iterations = attr.value;
					}

					Console.WriteLine("Number of iterations: " + iterations);

					var measures = new List<double>();

					for (int i = 0; i < iterations; i++)
					{
						stopWatch.Reset();
						stopWatch.Start();
						benchmark.Run();
						stopWatch.Stop();

						measures.Add(stopWatch.ElapsedTicks);
					}

					Console.WriteLine("Avg: {0}, Min: {1}, Max: {2}, Std: {3}", measures.Average(), measures.Min(), measures.Max(), Reporting.CalculateStdDev(measures));
					Console.WriteLine("------");

					benchmark.CleanUp();

					benchmarkResults.Add(benchmarkName, measures);
				}

				// For cosmetic reasons
				Console.WriteLine("");

				string csvBody = "";
				csvBody += benchmarkResults.Keys.Aggregate(";", (seed, benchmarkName) => seed + benchmarkName + ";");
				csvBody = csvBody.Substring(0, csvBody.Length - 1); // Remove last ';'
				csvBody += "\n";

				int benchmarkCount = benchmarkResults.Keys.Count;
				int maxIterations = benchmarkResults.Values.Select(measures => measures.Count).Max();

				for (int i = 0; i < maxIterations; i++)
				{
					string row = string.Format("{0}", i);

					foreach (var benchmarkName in benchmarkResults.Keys)
					{
						List<double> measures = benchmarkResults[benchmarkName];
						if (measures.Count > i)
						{
							row += string.Format(";{0}", measures[i]);
						}
						else
						{
							row += ";";
						}
					}

					csvBody += string.Format("{0}\n", row);
				}

				File.WriteAllText(outPath + "out.csv", csvBody);
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
		}
	}
}
