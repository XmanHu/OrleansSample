using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using GrainInterfaces;
using Grains;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime.Messaging;

namespace OrleansCPU
{
    class Program
    {
        private static int n = 250;
        private static int m = 15000;
        private static int blocksPerWorker = 30;
        private static int requestsPerBlock = 500;

        static async Task Main(string[] args)
        {
            //if (args.Length > 0)
            //{
            //    n = int.Parse(args[0]);
            //}

            var builder = new SiloHostBuilder()
                            .ConfigureDefaults()
                            .UseLocalhostClustering()
                            //.Configure<ConnectionOptions>(op => op.ProtocolVersion = NetworkProtocolVersion.Version2)
                            .ConfigureApplicationParts(parts =>
                            {
                                parts.AddApplicationPart(typeof(TestGrain).Assembly)
                                    .AddApplicationPart(typeof(ITestGrain).Assembly);
                            });

            var host = builder.Build();
            await host.StartAsync();

            var grainFactory = (IGrainFactory)host.Services.GetService(typeof(IGrainFactory));

            var grains = new ITestGrain[n];
            var tasks = new Task[n];

            for (int i = 0; i < n; i++)
            {
                grains[i] = grainFactory.GetGrain<ITestGrain>(Guid.NewGuid().GetHashCode());
                //grains2[i] = grainFactory.GetGrain<ITestGrain>(Guid.NewGuid().GetHashCode());
                //await grains[i].Test();
                tasks[i] = RunWorker(grains[i], requestsPerBlock, 3);
            }

            await Task.WhenAll(tasks);
            GC.Collect();
            GC.Collect();
            GC.Collect();

            var times = 3;

            while (times -->0)
            {
                var sw = new Stopwatch();
                sw.Start();

                for (int i = 0; i < n; i++)
                {
                    tasks[i] = RunWorker(grains[i], requestsPerBlock, blocksPerWorker);
                }

                //await Task.WhenAll(tasks.ToArray());
                await Task.WhenAll(tasks);
                sw.Stop();
                Console.WriteLine($"{n * requestsPerBlock * blocksPerWorker / sw.Elapsed.TotalSeconds} req /s, {n * requestsPerBlock * blocksPerWorker} reqs, using {sw.Elapsed.TotalSeconds}");
                //Console.WriteLine($"{n * 15000 / sw.Elapsed.TotalSeconds} req /s, {n * requestsPerBlock * blocksPerWorker} reqs, using {sw.Elapsed.TotalSeconds}");

                GC.Collect();
                GC.Collect();
                GC.Collect();
            }

            //var ctx = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            //int index = 0;

            //while (!ctx.IsCancellationRequested)
            //{
            //    grains[index].Add(ctx.Token);
            //    index = (index + 1) % 12;
            //}

            //var total = 0;
            //for (int i = 0; i < n; i++)
            //{
            //    total += await grains[i].GetCount();
            //}

            //Console.WriteLine(total);
            //Console.WriteLine(n*m / sw.Elapsed.TotalSeconds);

        }

        private static async Task RunWorker(ITestGrain state, int requestsPerBlock, int numBlocks)
        {
            //while (numBlocks > 0)
            //{
            //    for (int i = 0; i < requestsPerBlock; i++)
            //    {
            //        await state.Test().ConfigureAwait(false);
            //    }
            //    --numBlocks;
            //}

            //if (state2 != null)
            //{
            //    await state.InternalPing(state2, requestsPerBlock * numBlocks).ConfigureAwait(false);
            //}
            //else
            {
                for (int i = 0; i < requestsPerBlock * numBlocks; i++)
                {
                    await state.ImmuteTest(new MyTestType()).ConfigureAwait(false);
                }
            }


        }
    }
}
