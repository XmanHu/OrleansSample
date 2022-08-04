using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using GrainInterfaces;
using Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Runtime.Messaging;
using Orleans.Runtime.Placement;
using Orleans.Serialization;
using Orleans.Serialization.ProtobufNet;

namespace SecondSilo
{
    class Program
    {
        private static int siloPort = 11111;
        private static int gatewayPort = 30000;
        private static string ip = "127.0.0.1";
        static async Task<int> Main(string[] args)
        {
            try
            {
                int num = 1;
                if (args.Length > 0)
                {
                    ip = args[0];
                }

                var host = await StartSilo(num);
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");

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

        private static async Task<ISiloHost> StartSilo(int num)
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                    .ConfigureDefaults()
                    .UseDevelopmentClustering(IPEndPoint.Parse($"{ip}:11111"))
                    //.UseLocalhostClustering(primarySiloEndpoint: IPEndPoint.Parse("127.0.0.1:11111"))
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansSimple";
                })
                .ConfigureEndpoints(siloPort + num, gatewayPort + num, listenOnAnyHostAddress: true)
                .Configure<EndpointOptions>(options =>
                {
                    Console.WriteLine(options.AdvertisedIPAddress);
                    Console.WriteLine(options.SiloPort);
                    Console.WriteLine(options.SiloListeningEndpoint);

                    Console.WriteLine(options.GatewayPort);
                    Console.WriteLine(options.GatewayListeningEndpoint);
                })
                .Configure<ConnectionOptions>(op => op.ProtocolVersion = NetworkProtocolVersion.Version2)
                .Configure<SerializationProviderOptions>(op =>
                {
                    op.SerializationProviders.Add(typeof(ProtobufNetSerializer).GetTypeInfo());
                    op.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
                })
                //    .ConfigureLogging(logging => logging.AddConsole())
                ;

            var host = builder.Build();

            await host.StartAsync();
            return host;
        }
    }
}
