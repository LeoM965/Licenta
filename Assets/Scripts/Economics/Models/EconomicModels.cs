using System.Collections.Generic;

namespace Economics.Models
{
    public struct CropStats
    {
        public string VarietyName;
        public int TotalPlants;
        public int HarvestedPlants;
        public float FieldRevenue;
        public float HarvestedRevenue;
        public float TotalWeightKg;
        public float TotalSeedCost;
        public float TotalEnergyCost;
        public float TotalMaintenanceCost;
        public float TotalDepreciationCost;
        public float SoilFitSum;
        public int ParcelCount;

        public float TotalRevenue => FieldRevenue + HarvestedRevenue;
        public float TotalOperationalCost => TotalEnergyCost + TotalMaintenanceCost + TotalDepreciationCost;
        public float NetProfit => TotalRevenue - TotalSeedCost - TotalOperationalCost;
        public float ROI => (TotalSeedCost + TotalOperationalCost) > 0 ? (NetProfit / (TotalSeedCost + TotalOperationalCost)) * 100f : 0f;
        public float AvgSoilCompatibility => ParcelCount > 0 ? SoilFitSum / ParcelCount : 0f;

        public void AddPlantData(float weight, float marketPrice, float historicCost)
        {
            TotalWeightKg += weight;
            FieldRevenue += weight * marketPrice;
            TotalSeedCost += historicCost;
        }
    }

    public struct EconomicReport
    {
        public Dictionary<string, CropStats> AnalysisByVariety;
        public CropStats FarmTotals;
    }
}
