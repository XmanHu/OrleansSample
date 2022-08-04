using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NoOrleansCPU
{
    class Program
    {
        private static int n = 12;

        private static int cnt = 0;

        static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                n = int.Parse(args[0]);
            }

            var ctx = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var counts = new int[n];

            var tasks = new List<Task>();

            /*
            for (int i = 0; i < n; i++)
            {
                int p = i;
                tasks.Add(Task.Run(() =>
                {
                    while (!ctx.IsCancellationRequested)
                    {
                        counts[p]++;
                    }
                }));
            }
            */

            //Task.Run()

            await Task.WhenAll(tasks.ToArray());

            long total = 0;
            for(int i=0; i<n; i++)
            {
                total += counts[i];
                Console.WriteLine(counts[i]);
            }

            Console.WriteLine(total);
        }

    }



}
