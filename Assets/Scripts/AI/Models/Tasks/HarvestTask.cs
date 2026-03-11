using UnityEngine;

namespace AI.Core
{
    public class HarvestTask : RobotTask
    {
        public HarvestTask(Transform target, float gain) : base(target, gain, 0) { }
    }
}
