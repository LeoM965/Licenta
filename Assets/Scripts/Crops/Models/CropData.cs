using System;

[Serializable]
public class CropData
{
    public string name;
    public string prefabPath;
    public CropRequirements requirements;
    public float seedCostEUR;
    public float yieldWeightKg;
    public float marketPricePerKg;
    public int growthDays;

    public float yieldValueEUR => yieldWeightKg * marketPricePerKg;
    public float GrowthHours => growthDays * 24f;
}
