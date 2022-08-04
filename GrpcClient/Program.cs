using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Echo;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Helloworld;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GrpcClient
{
    class Program
    {
        private static int n = 500000;
        private static int m = 1;
        private static int k = 100;

        static async Task Main(string[] args)
        {
            //var clients = new List<CoHost.CoHost.CoHostClient>();
            var clients = new List<Greeter.GreeterClient>();
            var channels = new List<ChannelBase>();

            //var channel = new Channel("http://localhost:5001", ChannelCredentials.Insecure);


            //var ip = args.Length > 0 ? args[0] : "localhost:5001";
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            //var httpClientHandler = new HttpClientHandler();
            //httpClientHandler.UseProxy = false;
            //httpClientHandler.AllowAutoRedirect = false;
            //httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            //var channel = GrpcChannel.ForAddress($"http://{ip}", new GrpcChannelOptions
            //{
            //    HttpHandler = httpClientHandler,
            //    LoggerFactory = LoggerFactory.Create(c =>
            //    {
            //        c.AddConsole();
            //        c.SetMinimumLevel(LogLevel.Debug);
            //    })
            //});

            var ip = args.Length > 0 ? args[0] : "127.0.0.1:50051";
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            //var httpClientHandler = new HttpClientHandler();
            //httpClientHandler.UseProxy = false;
            //httpClientHandler.AllowAutoRedirect = false;
            //httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            //var channel = GrpcChannel.ForAddress($"http://{ip}", new GrpcChannelOptions() { HttpHandler = httpClientHandler });
            //var client = new CoHost.CoHost.CoHostClient(channel);

            //for (int i = 0; i < 10; i++)
            //{
            //    await client.EchoAsync(new EchoRequest());
            //}



            for (int i = 0; i < m; i++)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                //var httpClientHandler = new SocketsHttpHandler()
                //{
                //    EnableMultipleHttp2Connections = true,
                //    //SslOptions = new SslClientAuthenticationOptions
                //    //{
                //    //    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12,

                //    //    // TODO: Validate internal certificate
                //    //    RemoteCertificateValidationCallback = (_, _, _, _) => true // AcceptTestCertificates
                //    //},
                //    MaxConnectionsPerServer = 100
                //};
                var httpClientHandler = new HttpClientHandler();
                httpClientHandler.UseProxy = false;
                httpClientHandler.AllowAutoRedirect = false;
                httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                var channel = GrpcChannel.ForAddress($"http://{ip}", new GrpcChannelOptions()
                {
                    HttpClient = new HttpClient(httpClientHandler),
                    //DisposeHttpClient = true,
                    //LoggerFactory = NullLoggerFactory.Instance
                });
                //var client = new CoHost.CoHost.CoHostClient(channel);
                var client = new Greeter.GreeterClient(channel);
                clients.Add(client);
                //channels.Add(channel);
            }

            //n = args.Length > 1 ? Int32.Parse(args[1]) : n;

            //m = args.Length > 2 ? Int32.Parse(args[2]) : m;
            //k = args.Length > 3 ? Int32.Parse(args[3]) : k;
            //warm up
            var tasks = new List<Task>();

            for (int i = 0; i < m; i++)
            {
                var client = clients[i];

                //for (int p = 0; p < 10; p++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        for (int j = 0; j < 1000; j++)
                        {
                            //await client.PerTestAsync(new Empty());
                            await client.UnaryRequestAsync(new TestMessage
                            {
                                Id = Guid.NewGuid().ToString(),
                                Message = "message"
                            });
                        }
                    }));
                }
            }

            await Task.WhenAll(tasks);


            //perf test
            var sw = new Stopwatch();
            tasks.Clear();
            sw.Start();

            for (int i = 0; i < m; i++)
            {
                var client = clients[i];
                for (int p = 0; p < k; p++)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        for (int j = 0; j < n / k; j++)
                        {
                            //await client.PerTestAsync(new Empty());
                            await client.UnaryRequestAsync(new TestMessage
                            {
                                Id = Guid.NewGuid().ToString(),
                                Message = "message"
                            });
                        }
                    }));
                }

            }

            await Task.WhenAll(tasks);
            sw.Stop();

            Console.WriteLine(n * m / sw.Elapsed.TotalSeconds);

            //var client = new CoHost.CoHost.CoHostClient(channel);
            //var reply = await client.EchoAsync(new EchoRequest { Name = "Jialiang" });

            ////await client.PerTestAsync(new Empty());
            //Console.WriteLine(reply.Message);
        }
    }
}
