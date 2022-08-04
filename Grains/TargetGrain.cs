using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;

namespace Grains
{
    public class TargetGrain: Grain, ITargetGrain
    {
        public Task Work()
        {
            //var output = new MyTestType { num = input.num };
            return Task.CompletedTask;
            //return Task.FromResult(output);
        }
    }
}
