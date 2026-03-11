using UnityEngine;

namespace AI.Core
{
    public class FertilizationTask : RobotTask
    {
        public FertilizationTask(Transform target, float gain, float cost) : base(target, gain, cost) { }
    }
}
