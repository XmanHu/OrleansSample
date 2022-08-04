using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Echo;
using GrainInterfaces;
using Orleans;

namespace Grains
{
    //[SamplePlacementStrategy]
    public class EchoGrain: Grain, IEchoGrain
    {
        public Task<EchoReply> Echo(EchoRequest input)
        {
            //Console.WriteLine($"Receive protobuf msg from {input.Name} {this.GetPrimaryKey()}");
            return Task.FromResult(new EchoReply{ Message = input.Name});
        }
    }
}
