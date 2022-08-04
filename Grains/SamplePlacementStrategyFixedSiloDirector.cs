using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.Placement;
using Orleans.Runtime;
using Orleans.Runtime.Placement;

namespace Grains
{
    public class SamplePlacementStrategyFixedSiloDirector: IPlacementDirector
    {
        public Task<SiloAddress> OnAddActivation(Orleans.Runtime.PlacementStrategy strategy, PlacementTarget target, IPlacementContext context)
        {
            var silos = context.GetCompatibleSilos(target).OrderBy(s => s).ToArray();
            int silo = GetSiloNumber(target.GrainIdentity.PrimaryKey, silos.Length);
            return Task.FromResult(silos[silo]);
        }

        private int GetSiloNumber(Guid grainIdentityPrimaryKey, int silosLength)
        {
            return grainIdentityPrimaryKey.GetHashCode() % silosLength;
        }
    }

    [Serializable]
    public class SamplePlacementStrategy : PlacementStrategy
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class SamplePlacementStrategyAttribute : PlacementAttribute
    {
        public SamplePlacementStrategyAttribute() :
            base(new SamplePlacementStrategy())
        {
        }
    }
}
