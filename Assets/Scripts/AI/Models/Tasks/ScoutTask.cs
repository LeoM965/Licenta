using UnityEngine;

namespace AI.Core
{
    public class ScoutTask : RobotTask
    {
        public ScoutTask(Transform target, float gain) : base(target, gain, 0) { }
    }
}
