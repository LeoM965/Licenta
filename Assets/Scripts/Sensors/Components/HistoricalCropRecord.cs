namespace Sensors.Components
{
    public class HistoricalCropRecord
    {
        public int totalPlants;
        public float totalRevenue;
        public float totalWeightKg;
        public float totalSeedCost;

        public float Profit => totalRevenue - totalSeedCost;
    }
}
