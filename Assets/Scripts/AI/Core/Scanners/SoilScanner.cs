using UnityEngine;
using System.Collections.Generic;
using Sensors.Components;
using Sensors.Models;
using Sensors.Services;

namespace AI.Core.Scanners
{
    [CreateAssetMenu(fileName = "SoilScanner", menuName = "AI/Scanners/Soil Scanner")]
    public class SoilScanner : BaseScanner
    {
        public override void Scan(List<RobotTask> tasks, FenceZone[] zones)
        {
            if (ParcelCache.Instance == null) return;

            foreach (var parcel in ParcelCache.Instance.ParcelsIterator)
            {
                if (parcel == null || parcel.composition == null || parcel.isScheduledForTask)
                    continue;

                SoilAnalysis analysis = parcel.LatestAnalysis;
                if (analysis.HasAlerts)
                {
                    int zoneIdx = GetOrCreateZoneIndex(parcel, zones);
                    if (zoneIdx >= 0)
                    {
                        RobotTask task = CreateTask(parcel, analysis);
                        tasks.Add(task);
                        parcel.isScheduledForTask = true;
                    }
                }
            }
        }

        private RobotTask CreateTask(EnvironmentalSensor parcel, SoilAnalysis analysis)
        {
            if (analysis.requiresIrrigation)
            {
                float gain = analysis.moistureDeficit * 0.5f;
                float cost = analysis.moistureDeficit * 0.05f;
                return new IrrigationTask(parcel.transform, gain, cost);
            }
            if (analysis.requiresFertilization)
            {
                float totalDeficit = analysis.nitrogenDeficit + analysis.phosphorusDeficit + analysis.potassiumDeficit;
                float gain = totalDeficit * 1.5f;
                float cost = totalDeficit * 1.2f;
                return new FertilizationTask(parcel.transform, gain, cost);
            }
            if (analysis.requiresLiming)
            {
                float gain = 10f; // Simplified fixed gain for pH correction
                float cost = 3f;
                return new LimingTask(parcel.transform, gain, cost);
            }
            
            return new ScoutTask(parcel.transform, 5f);
        }
    }
}
