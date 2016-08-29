using System;
using System.IO;
using Mono.Options;

namespace SimpleBench.Runner
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			string dllPath = null;
			string outPath = Directory.GetCurrentDirectory();

			OptionSet optionSet = new OptionSet
			{
				{ "dll=", "The path of DLL containing benchmarks", path => dllPath = path },
				{ "o|out=", "The output path for reports", path => outPath = path }
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

				Console.WriteLine("DLL Path: " + dllPath);
				Console.WriteLine("Out Path: " + outPath);
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
