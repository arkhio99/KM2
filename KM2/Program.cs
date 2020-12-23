using System;
using System.Collections.Generic;
using System.Linq;
using StatLib;

namespace KM2
{
    class Program
    {
        static IList<double> GenerateHomogenousPoisson(int n, double T, double lam)
        {
            var timesOfEvents = new List<double>();
            var rand = new Random();

            double t = 0;
            while (true)
            {
                double u = rand.NextDouble();
                t = t - 1 / lam * Math.Log(u);

                if (t > T)
                {
                    break;
                }
                else
                {
                    timesOfEvents.Add(t);
                }
            }

            var intervals = new double[timesOfEvents.Count];
            for (int i = timesOfEvents.Count - 1; i >= 0; i--)
            {
                intervals[i] = timesOfEvents[i] - (i == 0 ? 0 : timesOfEvents[i - 1]);
            }

            return intervals;
        }

        static IList<double> GenerageHeterogenousPoisson(int n, double T, double lam, Func<double, double> lamFunc)
        {
            var timesOfEvents = new List<double>();
            var rand = new Random();

            double t = 0;
            while (true)
            {
                double u = rand.NextDouble();
                t = t - 1 / lam * Math.Log(u);

                if (t > T)
                {
                    break;
                }

                double u2 = rand.NextDouble();
                
                if (u2 <= lamFunc(t) / lam)
                {
                    timesOfEvents.Add(t);
                }
            }

            var intervals = new double[timesOfEvents.Count];
            for (int i = timesOfEvents.Count - 1; i >= 0; i--)
            {
                intervals[i] = timesOfEvents[i] - (i == 0 ? 0 : timesOfEvents[i - 1]);
            }

            return intervals;
        }

        static int SterjesNumber(int n)
        {
            return (int)(1 + 3.32218 * Math.Log10(n));
        }

        static double PearsonsNumber(IList<double> distribution, out int k)
        {
            double max = distribution.Max() + 0.1;
            double min = distribution.Min();
            int howIntervals = SterjesNumber(distribution.Count);
            double h = (max - min) / howIntervals;
            var averages = Enumerable.Range(1, howIntervals).Select( i => min + i * h - h / 2);
            var freqs = Enumerable.Range(1, howIntervals).Select(i => 0).ToArray();
            foreach(var val in distribution)
            {
                freqs[(int)Math.Floor((val - min) / h)]++;
            }

            double average = Statistics.GetSampleAverage(averages, freqs, distribution.Count);
            double lam = 1 / average;
            var theorProbs = from val in averages select Math.Exp(-lam * (val - h / 2)) - Math.Exp(-lam * (val + h / 2));
            var theorFreqs = theorProbs.Select(p => p * distribution.Count);
            k = howIntervals - 2;
            return Statistics.GetPearsonNumber(freqs, theorFreqs);
        }

        static void Main(string[] args)
        {
            var homogen = GenerateHomogenousPoisson(10, 10, 1);
            Console.WriteLine("Однородный процесс:");
            Console.WriteLine(homogen.StringView());
            var crit = PearsonsNumber(homogen, out int k);
            Console.WriteLine($"Критерий Пирсона = {crit}\nКоличество степеней свободы = {k}");

            Console.WriteLine("Неоднородный процесс:");
            var heterogen = GenerageHeterogenousPoisson(10, 10, 1, t => 2 / (t + 1));
            Console.WriteLine(heterogen.StringView());
            crit = PearsonsNumber(heterogen, out k);
            Console.WriteLine($"Критерий Пирсона = {crit}\nКоличество степеней свободы = {k}");

        }
    }
}
