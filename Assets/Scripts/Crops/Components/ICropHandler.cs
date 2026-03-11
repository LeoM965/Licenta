public interface ICropHandler
{
    void Initialize(float growthTime, float seedCost);
    void ManualUpdate(float currentTotalHours);
    void Harvest();
    bool IsFullyGrown { get; }
    bool IsBeingHarvested { get; }
    float Progress { get; }
}
