using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiResort_Simulator;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Simulator simulator = new Simulator();
            Loader.Load(Console.ReadLine(), simulator);

            bool simulate = true;
            while (simulate)
            {
                simulate = simulator.PerformSimulationStep();
                MessageFeed.PrintNews(Console.WriteLine);
                Console.ReadKey();
            }

            Console.ReadKey();
        }
    }
}
