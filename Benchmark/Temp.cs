//using System.Collections.Generic;
//using Google.Protobuf.WellKnownTypes;

//namespace Benchmark
//{
//    public class Temp
//    {
        
//    }

//    //External API:
//    interface IResourcePool
//    {
//        bool Matches(ITask task);  // if the task can run on current resource pool now
//        void Reserve(ITask task); // We reserve resource for this task in current resource pool
//        bool HasFreeResource(); // If this resource pool has free resource for this scheduling round
//        void Assign(ITask task); // Run task in the resource pool
//    }
//    interface ITasks : IEnumerable<ITask> { } // Priority Queue
//    interface ITask
//    {
//        IEnumerable<IResourceUnit> Required { get; }
//        void MarkInvalid(); // Set the task as not possible to match

//    }

//    interface IResourceUnit { }
//    interface IResource
//    {
//        IEnumerable<IResourceUnit> TotalSlots { get; }
//        IEnumerable<IResourceUnit> AssignedSlots { get; }
//        IEnumerable<IResourceUnit> ReservedSlots { get; }

//        void MarkForAssigned(ITask task);
//        void MarkForReserved(ITask task);
//        bool IsAvailableFor(ITask task);
//    }

//    // Provided API:
//    interface ISchedulingPolicy
//    {
//        void Schedule(ITasks tasks, IResourcePool resourcePool); // A scheduling round    
//    }
//    // FIFO implementation
//    class FIFOSchedulingPolicy : ISchedulingPolicy {
//        public void Schedule(ITasks tasks, IResourcePool resourcePool)
//        {
//            foreach (var task in tasks)
//            {
//                if (!resourcePool.HasFreeResource())
//                {
//                    return;
//                }
//                if (resourcePool.Matches(task))
//                {
//                    resourcePool.Assign(task);
//                }
//                else
//                {
//                    resourcePool.Reserve(task);
//                }
//            }
//        }
//    }
//    //pseudo code:

//    // Spread Homogeneous Resource Pool implementation:
//    public class SpreadResourcePool : IResourcePool
//    {
//        Queue<IResource> Resources; // sort by available slot descending

//        public bool Matches(ITask task)
//        {
//            return Resources.Peek().IsAvailableFor(task);
//        }

//        bool HasFreeResource()
//        {
//            return Resources.Peek().IsAvailableFor(Empty);
//        }

//        void Assign(ITask task)
//        {
//            var resource = Resources.Dequeue();
//            resource.MarkForAssigned(task.Required);
//            Resources.Enqueue(resource);
//        }

//        void Reserve(ITask task)
//        {
//            // For spread policy, reservation happens on first resource as well (also is easiest way)
//            // there is also an idea to improve that largest match least waste startegy. 
//            var resource = Resources.Dequeue();
//            resource.MarkForAssigned(task.Required);
//            Resources.Enqueue(resource);
//        }
//    }
//    // Further Improvement:
//    // - Implement Preemption:
//    class PreemptableResourcePool
//    { 
//    ...
//    bool Matches(ITask task);  // First check free resources, then check preemptable resources which run tasks with lower priority
//        bool HasFreeResource(); // Now also count preemptable resource as free resource
//    ...
//}
//    // - Use Backfill to Reduce Wastage Caused by Reservation
//    class BackfillSchedulingPolicy : ISchedulingPolicy
//    {
//        private ISchedulingPolicy basicPolicy;
//        private void BackfillSchedule(ITasks tasks, IResourcePool resourcePool) { ...} // Schedule tasks not at the head of the queue but are preemptable on reserved resources
//        public void Schedule(ITasks tasks, IResourcePool resourcePool)
//        {
//            basicPolicy.Schedule(tasks, resourcePool);
//            BackfillSchedule(tasks, resourcePool);
//        }
//    }
//    // - Leverage swimlane idea
//    interface ISwimlaneTask : ITask
//    {
//        int SwimlaneHash { get; } // indicate which swimlane this task is in
//    }
//    interface ISwimlaneResourcePool : IResourcePool
//    {
//        Set<int> SwimlaneHashes { get; } // which swimlanes this resource pool can support
//        bool Matches(ISwimlaneResourcePool task) => if(SwimlaneHashes.Contains(task.SwimlaneHash))? base.Match(task) : false;
//}

//}