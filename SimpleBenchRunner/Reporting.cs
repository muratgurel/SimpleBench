using System;
using System.Collections.Generic;
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
	}
}

