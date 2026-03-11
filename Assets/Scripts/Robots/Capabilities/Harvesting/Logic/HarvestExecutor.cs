using UnityEngine;
using Sensors.Components;

public class HarvestExecutor
{
    private EnvironmentalSensor parcel;
    private int harvestedInParcel;
    private float harvestTimer;
    private float harvestDelay;

    public int HarvestedInParcel => harvestedInParcel;

    public void SetTarget(EnvironmentalSensor targetParcel, float delay)
    {
        parcel = targetParcel;
        harvestDelay = delay;
        harvestedInParcel = 0;
    }

    public bool UpdateHarvest(CropGrowth crop, Transform robotTransform)
    {
        if (crop == null || crop.IsBeingHarvested || !crop.IsFullyGrown) return true;

        harvestTimer += Time.deltaTime;
        if (harvestTimer >= harvestDelay)
        {
            // Report revenue before destroying the crop information context
            ReportRevenue(crop, robotTransform);

            crop.Harvest();
            harvestedInParcel++;
            harvestTimer = 0f;
            return true;
        }
        return false;
    }

    private void ReportRevenue(CropGrowth crop, Transform robot)
    {
        if (Economics.Managers.RobotEconomicsManager.Instance == null) return;

        var sensor = crop.GetComponentInParent<EnvironmentalSensor>();
        if (sensor == null || string.IsNullOrEmpty(sensor.plantedVarietyName)) return;

        var db = CropLoader.Load();
        var data = db?.Get(sensor.plantedVarietyName);
        if (data != null)
        {
            int cropIndex = db.GetIndex(sensor.plantedVarietyName);
            float soilQuality = sensor.LatestAnalysis.qualityScore / 100f;

            var yWeights = Settings.SimulationSettings.YieldWeights;
            var mPrices = Settings.SimulationSettings.MarketPrices;

            float baseWeight = (yWeights != null && cropIndex >= 0 && cropIndex < yWeights.Length)
                ? yWeights[cropIndex] : data.yieldWeightKg;
            float marketPrice = (mPrices != null && cropIndex >= 0 && cropIndex < mPrices.Length)
                ? mPrices[cropIndex] : data.marketPricePerKg;

            float weight = baseWeight * soilQuality * crop.Progress;
            float revenue = marketPrice * weight;
            float seedCost = crop.PurchasePrice;

            Economics.Managers.RobotEconomicsManager.Instance.AddRobotRevenue(robot, revenue);
            sensor.RecordHarvest(weight, revenue, seedCost);
        }
    }

    public void Reset()
    {
        parcel = null;
        harvestedInParcel = 0;
    }
}
