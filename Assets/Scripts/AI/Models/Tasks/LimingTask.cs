using UnityEngine;

namespace AI.Core
{
    public class LimingTask : RobotTask
    {
        public LimingTask(Transform target, float gain, float cost) : base(target, gain, cost) { }
    }
}
