using System;
using System.Threading;
using System.Threading.Tasks;
using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace Grains
{
    //[SamplePlacementStrategy]
    public class HelloGrain : Grain, IHello
    {
        //private readonly ILogger logger;
        //private int count;

        //public HelloGrain(ILogger<HelloGrain> logger)
        //{
        //    this.logger = logger;
        //    this.count = 0;
        //}

        //public override Task OnActivateAsync()
        //{
        //    logger.LogInformation($"\n On active thread = { AppDomain.GetCurrentThreadId() } {this.GetPrimaryKey()} {count}");
        //    return base.OnActivateAsync();
        //}

        //public override Task OnDeactivateAsync()
        //{
        //    logger.LogInformation($"\n On deactive thread = { AppDomain.GetCurrentThreadId() } {this.GetPrimaryKey()} {count}");
        //    return base.OnDeactivateAsync();
        //}

        public Task<OutgoingMessage> SayHello(IncomeMessage greeting)
        {
            //logger.LogInformation($"\n SayHello message received: greeting = '{greeting}' thread = { AppDomain.GetCurrentThreadId() } {this.GetPrimaryKey()} {Interlocked.Increment(ref count)}");
            //if (this.GetPrimaryKeyLong() == 0)
            //{
                return Task.FromResult(new OutgoingMessage{ msg = greeting.msg});
            //}
            //else
            //{
            //    var grain = this.GrainFactory.GetGrain<IHello>(0);
            //    return grain.SayHello(greeting);
            //}
        }
    }

    public class HelloGrainState
    {
    }
}
