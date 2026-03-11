namespace Settings
{
    public static class SimulationSettings
    {
        public static int PlantsPerRow = 4;
        public static int SelectedCropIndex = -1;
        public static float[] SeedCosts;
        public static float[] YieldWeights;
        public static float[] MarketPrices;
        public static float EnergyPrice = 0.20f;

        public static void InitFromDatabase(CropDatabase db)
        {
            if (db?.crops == null) return;
            int n = db.crops.Length;
            SeedCosts = new float[n];
            YieldWeights = new float[n];
            MarketPrices = new float[n];
            for (int i = 0; i < n; i++)
            {
                SeedCosts[i] = db.crops[i].seedCostEUR;
                YieldWeights[i] = db.crops[i].yieldWeightKg;
                MarketPrices[i] = db.crops[i].marketPricePerKg;
            }
        }
    }
}
