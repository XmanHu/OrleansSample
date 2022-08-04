using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Echo;
using GrainInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime.Messaging;
using Orleans.Serialization;
using Orleans.Serialization.ProtobufNet;

namespace OrleansClient
{
    class Program
    {
        private static int n = 100000;
        private static int m = 100;
        static List<IMyStatelessWorkerGrain> grains = new List<IMyStatelessWorkerGrain>();
        private static ITestGrain[] perfGrains;
        private static Random rand = new Random();

        static async Task Main(string[] args)
        {
            var ip = args.Length > 0 ? args[0] : "127.0.0.1";
            if (args.Length > 1)
            {
                m = int.Parse(args[1]);
            }

            if (args.Length > 2)
            {
                n = int.Parse(args[2]);
            }

            try
            {
                var client = await ConnectClient(ip);
                await PerfWarmUp(client);
                await PerfTest();
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task PerfTest()
        {
            var sw = new Stopwatch();

            sw.Start();

            var tasks = new List<Task>();

            for (int j = 0; j < m; j++)
            {
                var count = j;
                tasks.Add(Task.Run(async () =>
                {
                    for (int i = 0; i < n; i++)
                    {
                        await perfGrains[count].Test().ConfigureAwait(false);
                    }
                }));
            }

            await Task.WhenAll(tasks.ToArray());
            sw.Stop();

            Console.WriteLine("perfTest: " + n * m / sw.Elapsed.TotalSeconds);
        }

        private static async Task PerfWarmUp(IClusterClient client)
        {
            var tasks = new List<Task>();
            perfGrains = new ITestGrain[m];

            for (int j = 0; j < m; j++)
            {
                var grain = client.GetGrain<ITestGrain>(j);
                perfGrains[j] = grain;

                tasks.Add(Task.Run(async () =>
                {
                    for (int i = 0; i < n / 10; i++)
                    {
                        await grain.Test().ConfigureAwait(false);
                    }
                }));
            }

            await Task.WhenAll(tasks.ToArray());

            GC.Collect();
            GC.Collect();
        }

        private static async Task TestProtoNet(IClusterClient client)
        {
            var grain = client.GetGrain<IProtoNetGrain>(rand.Next());
            var response = await grain.SayHello(new MyRequest{ msg = "Jialiang"});

            Console.WriteLine(response.msg);
        }

        private static async Task TestProto(IClusterClient client)
        {
            var grain = client.GetGrain<IEchoGrain>(rand.Next());
            var response = await grain.Echo(new EchoRequest{ Name = "Jialiang"});

            Console.WriteLine(response.Message);
        }

        private static async Task StatelessWarmUp(IClusterClient client)
        {
            var tasks = new List<Task>();

            for (int j = 0; j < m; j++)
            {
                var grain = client.GetGrain<IMyStatelessWorkerGrain>(Guid.Empty);
                grains.Add(grain);

                tasks.Add(Task.Run(async () =>
                {
                    for (int i = 0; i < n/10; i++)
                    {
                        await grain.Process().ConfigureAwait(false);
                    }
                }));
            }

            await Task.WhenAll(tasks.ToArray());

            GC.Collect();
            GC.Collect();
        }

        private static async Task TestHello(IClusterClient client)
        {
            var grain = client.GetGrain<IHello>(rand.Next());
            var response = await grain.SayHello(new IncomeMessage{ msg = "Hello Jialiang"});

            Console.WriteLine(response);
        }

        private static async Task StatelessPerf()
        {
            var sw = new Stopwatch();

            sw.Start();

            var tasks = new List<Task>();

            for (int j = 0; j < m; j++)
            {
                var count = j;
                tasks.Add(Task.Run(async () =>
                {
                    for (int i = 0; i < n; i++)
                    {
                        await grains[count].Process().ConfigureAwait(false);
                    }
                }));
            }

            await Task.WhenAll(tasks.ToArray());
            sw.Stop();

            Console.WriteLine(n * m/ sw.Elapsed.TotalSeconds);
        }

        private static async Task<IClusterClient> ConnectClient(string ip)
        {
            var client = new ClientBuilder()
                .UseStaticClustering(new IPEndPoint(IPAddress.Parse(ip), 30000))
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansSimple";
                })
               // .ConfigureLogging(logging => logging.AddConsole())
                .Configure<ConnectionOptions>(op => op.ProtocolVersion = NetworkProtocolVersion.Version2)
                //.Configure<SerializationProviderOptions>(op =>
                //{
                //    op.SerializationProviders.Add(typeof(ProtobufNetSerializer).GetTypeInfo());
                //    op.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
                //})
                //.AddSimpleMessageStreamProvider("SMSProvider")
                .Build();

            //var test = client.ServiceProvider.GetRequiredService<SerializationManager>();


            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host\n");
            return client;
        }
    }
}
