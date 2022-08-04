using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Echo;
using Google.Protobuf.WellKnownTypes;
using GrainInterfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Orleans;

namespace CoHostSilo.Services
{
    public class EchoService : CoHost.CoHost.CoHostBase
    {
        //private readonly IGrainFactory _client;
        //private readonly IEchoGrain _grain;
        //private readonly IMyStatelessWorkerGrain _grain1;

        //public EchoService(IGrainFactory client)
        //{
        //    _client = client;
        //    _grain = _client.GetGrain<IEchoGrain>(0);
        //    _grain1 = client.GetGrain<IMyStatelessWorkerGrain>(Guid.Empty);
        //}
        public static int num = 2;
        public int x = 0;

        public override Task<EchoReply> Echo(EchoRequest request, ServerCallContext context)
        {
            //return  _grain.Echo(request);
            num++;
            x++;
            Console.WriteLine(num + " " + x);
            return Task.FromResult(new EchoReply());
        }

        public override async Task<Empty> PerTest(Empty request, ServerCallContext context)
        {
            //await _grain1.Process();
            return new Empty();
        }
    }
}
