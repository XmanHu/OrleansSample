using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface ITargetGrain: IGrainWithIntegerKey
    {
        public Task Work();
    }
}