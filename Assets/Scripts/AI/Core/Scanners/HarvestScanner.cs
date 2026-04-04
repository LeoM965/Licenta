using UnityEngine;
using System.Collections.Generic;
using Sensors.Components;

namespace AI.Core.Scanners
{
    [CreateAssetMenu(fileName = "HarvestScanner", menuName = "AI/Scanners/Harvest Scanner")]
    public class HarvestScanner : BaseScanner
    {
        public override void Scan(List<RobotTask> tasks, FenceZone[] zones)
        {
            if (ParcelCache.Instance == null) return;

            var db = CropLoader.Load();

            foreach (var parcel in ParcelCache.Instance.ParcelsIterator)
            {
                if (parcel == null || parcel.isScheduledForTask || parcel.activeCrops.Count == 0)
                    continue;

                float gain = GetHarvestValue(parcel, db);
                if (gain <= 0f) continue;

                int zoneIdx = GetOrCreateZoneIndex(parcel, zones);
                if (zoneIdx >= 0)
                {
                    float cost = GetEstimatedEnergyCost(parcel);
                    tasks.Add(new HarvestTask(parcel.transform, gain, cost));
                    parcel.isScheduledForTask = true;
                }
            }
        }

        private float GetHarvestValue(EnvironmentalSensor parcel, CropDatabase db)
        {
            float value = 0f;
            var cropData = db?.Get(parcel.plantedVarietyName);

            foreach (var crop in parcel.activeCrops)
            {
                if (crop == null || !crop.IsHarvestable) continue;
                value += cropData != null ? cropData.yieldValueEUR : 25f;
            }
            return value;
        }

        private float GetEstimatedEnergyCost(EnvironmentalSensor parcel)
        {
            int harvestable = 0;
            foreach (var crop in parcel.activeCrops)
                if (crop != null && crop.IsHarvestable) harvestable++;
            return harvestable * 0.5f;
        }
    }
}
