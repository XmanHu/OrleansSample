using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;

namespace Grains
{
    [StatelessWorker]
    public class MyStatelessWorkerGrain : Grain, IMyStatelessWorkerGrain
    {
        private readonly ILogger logger;
        private int count;

        public MyStatelessWorkerGrain(ILogger<MyStatelessWorkerGrain> logger)
        {
            this.logger = logger;
            this.count = 0;
        }

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

        public Task Process()
        {
            //logger.LogInformation($"\n message received: greeting = '{str}' thread = { AppDomain.GetCurrentThreadId() } {this.GetPrimaryKey()} {Interlocked.Increment(ref count)}");

            //await Task.Delay(5000);

            //return $"\n Client said: '{str}', so MyStatelessWorkerGrain says: Hello!";
            return Task.CompletedTask;
        }
    }
}
