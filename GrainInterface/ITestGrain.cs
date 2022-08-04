using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace GrainInterfaces
{
    public interface ITestGrain : IGrainWithIntegerKey
    {
        public Task<int> GetCount();
        public Task Test();
        public Task ImmuteTest(MyTestType input);

        //[AlwaysInterleave]
        //public Task InternalPing(ITestGrain grain, int count);
    }

    //[Immutable]
    public class MyTestType
    {
        public int num;
        public byte[] bytes = new byte[100];
    }
}
