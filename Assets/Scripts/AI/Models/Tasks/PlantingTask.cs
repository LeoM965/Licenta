using UnityEngine;

namespace AI.Core
{
    public class PlantingTask : RobotTask
    {
        public PlantingTask(Transform target, float gain, float cost) : base(target, gain, cost) { }
    }
}
