using System;
using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface IHello : IGrainWithIntegerKey
    {
        Task<OutgoingMessage> SayHello(IncomeMessage greeting);
    }

    public class IncomeMessage
    {
        public string msg;
    }

    public class OutgoingMessage
    {
        public string msg;
    }
}
