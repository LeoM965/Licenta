using UnityEngine;

namespace AI.Core
{
    public class HarvestTask : RobotTask
    {
        public HarvestTask(Transform target, float gain, float cost) : base(target, gain, cost) { }
    }
}
