using UnityEngine;
using System.Collections.Generic;
using Sensors.Components;

namespace AI.Core.Scanners
{
    [CreateAssetMenu(fileName = "PlantingScanner", menuName = "AI/Scanners/Planting Scanner")]
    public class PlantingScanner : BaseScanner
    {
        [SerializeField] private float minSoilQuality = 30f;
        
        public override void Scan(List<RobotTask> tasks, FenceZone[] zones)
        {
            if (ParcelCache.Instance == null) return;

            foreach (var parcel in ParcelCache.Instance.ParcelsIterator)
            {
                if (parcel == null || 
                    parcel.isScheduledForTask || 
                    parcel.activeCrops.Count > 0 || 
                    parcel.soilQuality < minSoilQuality) 
                {
                    continue;
                }

                int zoneIdx = GetOrCreateZoneIndex(parcel, zones);
                if (zoneIdx >= 0)
                {
                    // Prioritize based on soil quality - higher quality = higher NetValue
                    tasks.Add(new PlantingTask(parcel.transform, parcel.soilQuality)); 
                    parcel.isScheduledForTask = true;
                }
            }
        }
    }
}
