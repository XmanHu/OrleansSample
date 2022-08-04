using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using ProtoBuf;

namespace GrainInterfaces
{
    public interface IProtoNetGrain : IGrainWithIntegerKey
    {
        public Task<MyResponse> SayHello(MyRequest request);
    }


    [ProtoContract]
    [Serializable]
    public class MyRequest
    {
        [ProtoMember(1)]
        public string msg;
    }

    [ProtoContract]
    [Serializable]
    public class MyResponse
    {
        [ProtoMember(1)]
        public string msg;
    }
}
