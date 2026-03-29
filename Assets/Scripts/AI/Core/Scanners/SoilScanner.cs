using UnityEngine;
using System.Collections.Generic;

namespace AI.Core.Scanners
{
    [CreateAssetMenu(fileName = "SoilScanner", menuName = "AI/Scanners/Soil Scanner")]
    public class SoilScanner : BaseScanner
    {
        public override void Scan(List<RobotTask> tasks, FenceZone[] zones) { }
    }
}
