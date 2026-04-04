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

            var db = CropLoader.Load();
            float avgYieldValue = GetAverageYieldValue(db);
            float avgSeedCost = GetAverageSeedCost(db);

            foreach (var parcel in ParcelCache.Instance.ParcelsIterator)
            {
                if (parcel == null || parcel.isScheduledForTask || 
                    parcel.activeCrops.Count > 0 || parcel.soilQuality < minSoilQuality)
                    continue;

                int zoneIdx = GetOrCreateZoneIndex(parcel, zones);
                if (zoneIdx >= 0)
                {
                    float suitability = parcel.soilQuality / 100f;
                    float gain = suitability * avgYieldValue;
                    float cost = avgSeedCost;
                    tasks.Add(new PlantingTask(parcel.transform, gain, cost));
                    parcel.isScheduledForTask = true;
                }
            }
        }

        private float GetAverageSeedCost(CropDatabase db)
        {
            if (db?.crops == null || db.crops.Length == 0) return 5f;
            float total = 0f;
            foreach (var crop in db.crops) total += crop.seedCostEUR;
            return total / db.crops.Length;
        }

        private float GetAverageYieldValue(CropDatabase db)
        {
            if (db?.crops == null || db.crops.Length == 0) return 1f;
            float total = 0f;
            foreach (var crop in db.crops) total += crop.yieldValueEUR;
            return total / db.crops.Length;
        }
    }
}
