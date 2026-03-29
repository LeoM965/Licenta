using UnityEngine;

namespace AI.Core
{
    public class PlantingTask : RobotTask
    {
        public PlantingTask(Transform target, float gain) : base(target, gain, 0) { }
    }
}
