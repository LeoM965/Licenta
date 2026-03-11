using UnityEngine;

namespace AI.Core
{
    public class IrrigationTask : RobotTask
    {
        public IrrigationTask(Transform target, float gain, float cost) : base(target, gain, cost) { }
    }
}
