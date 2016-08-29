using System;
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

				var instances = benchmarkAssembly.GetTypes()
				                                 .Where(t => t.GetInterfaces().Contains(typeof(IBenchmark)) && 
				                                        t.GetConstructor(Type.EmptyTypes) != null)
				                                 .Select(t => Activator.CreateInstance(t) as IBenchmark);

				foreach (var instance in instances)
				{
					
				}
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
