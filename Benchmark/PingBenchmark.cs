using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using GrainInterfaces;
using Grains;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime.Messaging;

namespace Benchmark
{
    [MemoryDiagnoser]
    public class PingBenchmark : IDisposable 
    {
        private readonly List<ISiloHost> hosts = new List<ISiloHost>();
        private readonly ITestGrain grain;
        private readonly IClusterClient client;

        public PingBenchmark() : this(1, true) { }

        public PingBenchmark(int numSilos, bool startClient, bool grainsOnSecondariesOnly = false)
        {
            for (var i = 0; i < numSilos; ++i)
            {
                var primary = i == 0 ? null : new IPEndPoint(IPAddress.Loopback, 11111);
                var siloBuilder = new SiloHostBuilder()
                    .ConfigureDefaults()
                    .UseLocalhostClustering(
                        siloPort: 11111 + i,
                        gatewayPort: 30000 + i,
                        primarySiloEndpoint: primary)
                    .Configure<ConnectionOptions>(op => op.ProtocolVersion = NetworkProtocolVersion.Version2);

                if (i == 0 && grainsOnSecondariesOnly)
                {
                    siloBuilder.ConfigureApplicationParts(parts =>
                        parts.AddApplicationPart(typeof(ITestGrain).Assembly));
                    siloBuilder.ConfigureServices(services =>
                    {
                        services.Remove(services.First(s => s.ImplementationType?.Name == "ApplicationPartValidator"));
                    });
                }
                else
                {
                    siloBuilder.ConfigureApplicationParts(parts =>
                        parts.AddApplicationPart(typeof(ITestGrain).Assembly)
                             .AddApplicationPart(typeof(TestGrain).Assembly));
                }

                var silo = siloBuilder.Build();
                silo.StartAsync().GetAwaiter().GetResult();
                this.hosts.Add(silo);
            }

            if (grainsOnSecondariesOnly) Thread.Sleep(4000);

            if (startClient)
            {
                var clientBuilder = new ClientBuilder()
                    .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(ITestGrain).Assembly))
                    .Configure<ClusterOptions>(options => options.ClusterId = options.ServiceId = "dev")
                    .Configure<ConnectionOptions>(op => op.ProtocolVersion = NetworkProtocolVersion.Version2);

                if (numSilos == 1)
                {
                    clientBuilder.UseLocalhostClustering();
                }
                else
                {
                    var gateways = Enumerable.Range(30000, numSilos).Select(i => new IPEndPoint(IPAddress.Loopback, i)).ToArray();
                    clientBuilder.UseStaticClustering(gateways);
                }

                this.client = clientBuilder.Build();
                this.client.Connect().GetAwaiter().GetResult();
                var grainFactory = this.client;

                this.grain = grainFactory.GetGrain<ITestGrain>(Guid.NewGuid().GetHashCode());
                this.grain.Test().GetAwaiter().GetResult();
            }
        }
        
        [Benchmark]
        public Task Ping() => grain.Test();

        public async Task PingForever()
        {
            while (true)
            {
                await grain.Test();
            }
        }

        public Task PingConcurrent() => this.Run(
            runs: 3,
            grainFactory: this.client,
            blocksPerWorker: 10);

        public Task PingConcurrentHostedClient(int blocksPerWorker = 30) => this.Run(
            runs: 3,
            grainFactory: (IGrainFactory)this.hosts[0].Services.GetService(typeof(IGrainFactory)),
            blocksPerWorker: blocksPerWorker);

        private async Task Run(int runs, IGrainFactory grainFactory, int blocksPerWorker)
        {
            var loadGenerator = new ConcurrentLoadGenerator<ITestGrain>(
                maxConcurrency: 100,
                blocksPerWorker: blocksPerWorker,
                requestsPerBlock: 500,
                issueRequest: g => g.Test(),
                getStateForWorker: workerId => grainFactory.GetGrain<ITestGrain>(Guid.NewGuid().GetHashCode()));
            await loadGenerator.Warmup();
            while (runs-- > 0) await loadGenerator.Run();
        }

        public async Task Shutdown()
        {
            if (this.client is IClusterClient c)
            {
                await c.Close();
                c.Dispose();
            }

            this.hosts.Reverse();
            foreach (var h in this.hosts)
            {
                await h.StopAsync();
                h.Dispose();
            }
        }

        [GlobalCleanup]
        public void Dispose()
        {
            (this.client as IDisposable)?.Dispose(); 
            this.hosts.ForEach(h => h.Dispose());
        }
    }
}