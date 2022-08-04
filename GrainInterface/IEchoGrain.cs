using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Echo;
using Orleans;

namespace GrainInterfaces
{
    public interface IEchoGrain : IGrainWithIntegerKey
    {
        public Task<EchoReply> Echo(EchoRequest input);
    }
}
