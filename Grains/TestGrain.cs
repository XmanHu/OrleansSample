using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;
using Orleans.Concurrency;

namespace Grains
{
    public class TestGrain: Grain, ITestGrain
    {
        private int counter = 0;
        private ITargetGrain target;
        private ITestGrain self;

        //public override Task OnActivateAsync()
        //{
        //    target = this.GrainFactory.GetGrain<ITargetGrain>(this.GetPrimaryKeyLong());
        //    this.self = this.AsReference<ITestGrain>();
        //    return base.OnActivateAsync();
        //}

        public Task Test()
        {
            return Task.CompletedTask;
        }

        //public async Task Add(CancellationToken token)
        //{
        //    await target.Work();
        //    //if (!token.IsCancellationRequested)
        //    //{
        //    //    counter++;
        //    //    //Thread.Sleep(500);
        //    //}

        //    //var g = this.GrainFactory.GetGrain<>();
        //    //return Task.CompletedTask;
        //}

        public Task<int> GetCount()
        {
            return Task.FromResult(counter);
            //return Task.CompletedTask;
        }

        public Task ImmuteTest(MyTestType input)
        {
            return Task.CompletedTask;
            //var output = new MyTestType { num = input.num };
            //return Task.CompletedTask;
            //return target.Work();
            //await target.Work().ConfigureAwait(false);
            //return Task.CompletedTask;
        }

        //public async Task InternalPing(ITestGrain grain, int count)
        //{
        //    //if (count == 0) return Task.CompletedTask;
        //    //return 
        //    //     grain.InternalPing(this.self, count - 1);
        //    for (int i = 0; i < count; i++)
        //    {
        //        await grain.Test();
        //    }
        //}
    }
}
