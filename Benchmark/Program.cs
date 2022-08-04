using System;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                Console.WriteLine("## Client to Silo ##");
                var test = new PingBenchmark(numSilos: 1, startClient: true);
                test.PingConcurrent().GetAwaiter().GetResult();
                test.Shutdown().GetAwaiter().GetResult();
            }
            GC.Collect();
            {
                Console.WriteLine("## Client to 2 Silos ##");
                var test = new PingBenchmark(numSilos: 2, startClient: true);
                test.PingConcurrent().GetAwaiter().GetResult();
                test.Shutdown().GetAwaiter().GetResult();
            }
            GC.Collect();
            {
                Console.WriteLine("## Hosted Client ##");
                var test = new PingBenchmark(numSilos: 1, startClient: false);
                test.PingConcurrentHostedClient().GetAwaiter().GetResult();
                test.Shutdown().GetAwaiter().GetResult();
            }
            GC.Collect();
            {
                // All calls are cross-silo because the calling silo doesn't have any grain classes.
                Console.WriteLine("## Silo to Silo ##");
                var test = new PingBenchmark(numSilos: 2, startClient: false, grainsOnSecondariesOnly: true);
                test.PingConcurrentHostedClient(blocksPerWorker: 10).GetAwaiter().GetResult();
                test.Shutdown().GetAwaiter().GetResult();
            }
        }
    }
}
