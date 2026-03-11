using System.Collections.Generic;
using Sensors.Components;

namespace AI.Core
{
    public interface ITaskScanner
    {
        void Scan(List<RobotTask> tasks, FenceZone[] zones);
    }
}
