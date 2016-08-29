using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleBench.Runner
{
	public static class Reporting
	{
		public static double CalculateStdDev(IEnumerable<double> values)
		{
			if (values.Any())
			{
				double avg = values.Average();
				double sum = values.Sum(d => Math.Pow(d - avg, 2));
				return Math.Sqrt((sum) / (values.Count() - 1));
			}

			return 0;
		}

		public static StringWriter ExportCSV(IEnumerable<BenchmarkResult> results)
		{
			var sWriter = new StringWriter();

			sWriter.WriteLine(results.Aggregate("", (seed, result) => ";" + seed + result.name));

			int maxIterations = results.Select(br => br.measures.Count).Max();

			for (int i = 0; i < maxIterations; i++)
			{
				sWriter.Write(i);

				foreach (var r in results)
				{
					List<double> measures = r.measures;
					sWriter.Write(string.Format(";{0}", (measures.Count > i) ? measures[i].ToString() : ""));
				}

				sWriter.Write("\n");
			}

			sWriter.Flush();
			return sWriter;
		}
	}
}

