using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Echo;
using GrainInterfaces;
using Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Runtime.Messaging;
using Orleans.Runtime.Placement;
using Orleans.Serialization;
using Orleans.Serialization.ProtobufNet;

namespace Silo
{
    class Program
    {
        private static int siloPort = 11111;
        private static int gatewayPort = 30000;
        private static int run = 3;
        private static int n = 15000;
        private static int m = 250;
        private static int blocksPerWorker = 30;
        private static int requestsPerBlock = 500;

        static async Task<int> Main(string[] args)
        {
            try
            {
                int num = 0;
                if (args.Length > 0)
                {
                    num = int.Parse(args[0]);
                }

                int defaultType = 0;
                if (args.Length > 1)
                {
                    defaultType = int.Parse(args[1]);
                }

                var host = await StartSilo(num);
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");

                Console.ReadLine();

                var grainfactory = host.Services.GetService<IGrainFactory>();

                switch (defaultType)
                {
                    case 0:
                        await DefaultTask(grainfactory);
                        break;
                    case 1:
                        await ProtobufTask(grainfactory);
                        break;
                    case 2:
                        await ProtoNetTask(grainfactory);
                        break;
                    case 3:
                        await ImmutableTask(grainfactory);
                        break;
                }

                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task DefaultTask(IGrainFactory grainFactory)
        {
            var tasks = new List<Task>();

            var grains = new IHello[m];
            for (int i = 0; i < m; i++)
            {
                grains[i] = grainFactory.GetGrain<IHello>(i);
                await grains[i].SayHello(new IncomeMessage());
            }

            while (run-- > 0)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (int i = 0; i < m; i++)
                {
                    int p = i;
                    tasks.Add(Task.Run(async () =>
                    {
                        for (int j = 0; j < n; j++)
                        {
                            await grains[p].SayHello(new IncomeMessage { msg = "foo" });
                        }
                    }));
                }

                await Task.WhenAll(tasks);
                sw.Stop();
                Console.WriteLine($"Throughput is {n * m / sw.Elapsed.TotalSeconds}");
                tasks.Clear();
                GC.Collect();
            }

        }

        private static async Task ProtobufTask(IGrainFactory grainFactory)
        {
            var tasks = new List<Task>();

            var grains = new IEchoGrain[m];
            for (int i = 0; i < m; i++)
            {
                grains[i] = grainFactory.GetGrain<IEchoGrain>(i);
                await grains[i].Echo(new EchoRequest());
            }

            while (run --> 0)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (int i = 0; i < m; i++)
                {
                    int p = i;
                    tasks.Add(Task.Run(async () =>
                    {
                        for (int j = 0; j < n; j++)
                        {
                            await grains[p].Echo(new EchoRequest { Name = "foo" });
                        }
                    }));
                }

                await Task.WhenAll(tasks.ToArray());
                sw.Stop();
                Console.WriteLine($"Throughput is {n * m / sw.Elapsed.TotalSeconds}");
                tasks.Clear();
                GC.Collect();
            }

        }

        private static async Task ProtoNetTask(IGrainFactory grainFactory)
        {
            var tasks = new List<Task>();

            var grains = new IProtoNetGrain[m];
            for (int i = 0; i < m; i++)
            {
                grains[i] = grainFactory.GetGrain<IProtoNetGrain>(i);
                await grains[i].SayHello(new MyRequest());
            }

            while (run -- > 0)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (int i = 0; i < m; i++)
                {
                    int p = i;
                    tasks.Add(Task.Run(async () =>
                    {
                        for (int j = 0; j < n; j++)
                        {
                            await grains[p].SayHello(new MyRequest { msg = "foo" });
                        }
                    }));
                }

                await Task.WhenAll(tasks.ToArray());
                sw.Stop();
                Console.WriteLine($"Throughput is {n * m / sw.Elapsed.TotalSeconds}");
                tasks.Clear();
                GC.Collect();
            }
        }

        private static async Task ImmutableTask(IGrainFactory grainFactory)
        {
            var grains = new ITestGrain[n];
            var tasks = new Task[n];

            for (int i = 0; i < n; i++)
            {
                grains[i] = grainFactory.GetGrain<ITestGrain>(Guid.NewGuid().GetHashCode());
                tasks[i] = RunWorker(grains[i], requestsPerBlock, 3);
            }

            await Task.WhenAll(tasks);
            GC.Collect();
            GC.Collect();
            GC.Collect();

            var times = 3;

            while (times-- > 0)
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
             
                GC.Collect();
                GC.Collect();
                GC.Collect();
            }
        }
        private static async Task RunWorker(ITestGrain state, int requestsPerBlock, int numBlocks)
        {
            for (int i = 0; i < requestsPerBlock * numBlocks; i++)
            {
                await state.ImmuteTest(new MyTestType()).ConfigureAwait(false);
            }
        }

        private static async Task<ISiloHost> StartSilo(int num)
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                   // .ConfigureDefaults()
                .UseLocalhostClustering()
                  //.UseDevelopmentClustering(IPEndPoint.Parse("127.0.0.1:11111"))
                    //primarySiloEndpoint:IPEndPoint.Parse("127.0.0.1:11111")
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansSimple";
                })
                .ConfigureEndpoints(siloPort + num, gatewayPort + num,listenOnAnyHostAddress:true)
                .Configure<EndpointOptions>(options =>
                {
                    //options.AdvertisedIPAddress = IPAddress.Loopback;
                    //options.GatewayPort = gatewayPort + num;
                    //options.SiloPort = siloPort + num;
                    Console.WriteLine(options.AdvertisedIPAddress);
                    Console.WriteLine(options.SiloPort);
                    Console.WriteLine(options.SiloListeningEndpoint);

                    Console.WriteLine(options.GatewayPort);
                    Console.WriteLine(options.GatewayListeningEndpoint);
                })
                .Configure<ConnectionOptions>(op =>  op.ProtocolVersion = NetworkProtocolVersion.Version2 )
                //.Configure<SerializationProviderOptions>(op =>
                //{
                //    op.SerializationProviders.Add(typeof(ProtobufNetSerializer).GetTypeInfo());
                //    op.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
                //})
                //.ConfigureApplicationParts(parts =>
                //{
                //    parts.AddApplicationPart(typeof(IEchoGrain).Assembly);
                //    parts.AddApplicationPart(typeof(IProtoNetGrain).Assembly);
                //    parts.AddApplicationPart(typeof(IHello).Assembly);
                //})
                //.ConfigureServices(services =>
                //    {
                //        services.Remove(services.First(s => s.ImplementationType?.Name == "ApplicationPartValidator"));
                //    })
                //.Configure<GrainCollectionOptions>(op => { op.CollectionAge = TimeSpan.FromMinutes(5); })
                //.ConfigureLogging(logging => logging.AddConsole())
                ;

            var host = builder.Build();

            await host.StartAsync();
            return host;
        }
    }
}