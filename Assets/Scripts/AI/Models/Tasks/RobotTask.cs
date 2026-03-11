using UnityEngine;

namespace AI.Core
{
    public abstract class RobotTask
    {
        public Transform Target { get; }
        public float PotentialGain { get; protected set; }
        public float ResourceCost { get; protected set; }
        public float NetValue => PotentialGain - ResourceCost;

        protected RobotTask(Transform target, float gain = 0, float cost = 0)
        {
            Target = target;
            PotentialGain = gain;
            ResourceCost = cost;
        }

        public override bool Equals(object obj) => obj is RobotTask other && Target == other.Target;
        public override int GetHashCode() => Target != null ? Target.GetHashCode() : 0;
    }
}
