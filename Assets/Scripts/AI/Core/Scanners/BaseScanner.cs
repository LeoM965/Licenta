using UnityEngine;
using System.Collections.Generic;
using Sensors.Components;

namespace AI.Core.Scanners
{
    public abstract class BaseScanner : ScriptableObject, ITaskScanner
    {
        public abstract void Scan(List<RobotTask> tasks, FenceZone[] zones);
        
        protected int GetOrCreateZoneIndex(EnvironmentalSensor parcel, FenceZone[] zones)
        {
            if (parcel.zoneIndex != -1) return parcel.zoneIndex;
            if (zones == null) return -1;

            FenceZone zone = BoundsHelper.FindZoneContaining(parcel.transform.position, zones);
            if (zone != null)
            {
                parcel.zoneIndex = System.Array.IndexOf(zones, zone);
            }
            return parcel.zoneIndex;
        }
    }
}
