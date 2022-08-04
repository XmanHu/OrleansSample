using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using CoHostSilo.Services;
using Grains;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.Configuration;
using Orleans.Runtime.Messaging;
using Orleans.Serialization;

namespace CoHostSilo
{
    public class Program
    {
        private static string ip = "127.0.0.1";
        private static int port = 5001;
        private static string protocol = "h2c";

        public static Task Main(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.UseOrleans(siloBuilder =>
                //{
                //    siloBuilder
                //        .UseLocalhostClustering()
                //        .Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromMinutes(1))
                //        .Configure<ClusterOptions>(opts =>
                //        {
                //            opts.ClusterId = "dev";
                //            opts.ServiceId = "OrleansSimple";
                //        })
                //        .Configure<ConnectionOptions>(op => op.ProtocolVersion = NetworkProtocolVersion.Version2)
                //        .Configure<SerializationProviderOptions>(op =>
                //        {
                //            op.SerializationProviders.Add(typeof(ProtobufSerializer).GetTypeInfo());
                //        })
                //        .Configure<EndpointOptions>(opts =>
                //        {
                //            opts.AdvertisedIPAddress = IPAddress.Loopback;
                //        })
                //        //.ConfigureLogging(loggerFactory =>
                //        //{
                //        //    loggerFactory.ClearProviders();
                //        //})
                //        ;
                //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                        webBuilder.Configure((ctx, app) =>
                        {
                            //app.UseHttpsRedirection();
                            app.UseRouting();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapGrpcService<EchoService>();
                                endpoints.MapControllers();
                            });
                        });
                    //webBuilder.UseUrls("https://*:5001");

                    webBuilder.ConfigureKestrel((context, options) =>
                    {

                        // ListenAnyIP will work with IPv4 and IPv6.
                        // Chosen over Listen+IPAddress.Loopback, which would have a 2 second delay when
                        // creating a connection on a local Windows machine.
                        options.ListenAnyIP(port, listenOptions =>
                        {
                            Console.WriteLine($"Address: {ip}:{port}, Protocol: {protocol}");


                            listenOptions.Protocols = HttpProtocols.Http2;
                            //listenOptions.UseHttps("D:\\WorkStation\\Orleans\\OrleansSample\\Certs\\testCert.pfx", "testPassword", httpsOptions =>
                            //{
                            //    httpsOptions.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.AllowCertificate;
                            //    httpsOptions.AllowAnyClientCertificate();
                            //});

                        });
                    });
                })
                .ConfigureServices(services =>
                {
                    services.AddControllers();
                    services.AddGrpc(o => o.IgnoreUnknownServices = true);
                    services.Configure<RouteOptions>(c => c.SuppressCheckForUnhandledSecurityMetadata = true);
                    //services.AddSingleton<EchoService>();
                })
                .ConfigureLogging(loggerFactory =>
                {
                    loggerFactory.ClearProviders();
                })
                .RunConsoleAsync();
    }
}
