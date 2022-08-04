using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;

namespace Grains
{
    public class ProtoNetGrain : Grain, IProtoNetGrain
    {
        private readonly ILogger logger;

        public ProtoNetGrain(ILogger<ProtoNetGrain> logger)
        {
            this.logger = logger;
        }

        public Task<MyResponse> SayHello(MyRequest request)
        {
            //this.logger.LogInformation($"Received request {request.msg}");
            return Task.FromResult(new MyResponse {msg = request.msg});
        }
    }
}
