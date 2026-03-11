using UnityEngine;
using System.Collections.Generic;
using Sensors.Components;

public class CropPlanter : RobotOperator
{
    [SerializeField] private PlantingConfig config = new PlantingConfig();
    
    private PlanterOperation operation;

    protected override void Start()
    {
        base.Start();
        CropDatabase cropDB = CropLoader.Load();
        operation = new PlanterOperation(transform, movement, energy, config, cropDB);
        Invoke(nameof(Initialize), 3f);
    }
    
    private void Initialize()
    {
        FenceZone zone = ZoneHelper.GetZoneAt(transform.position);
        parcels = ParcelHelper.GetParcelsInZone(zone, config.minSoilQuality);
        parcelIndex = 0;
        if (parcels.Count > 0) MoveToNextParcel();
    }

    protected override void UpdateOperation() => operation.Update();
    protected override bool IsWorking() => operation.IsPlanting;
    protected override float GetArriveDistance() => config.arriveDistance;

    protected override void OnArrivedAtParcel(EnvironmentalSensor parcel)
    {
        operation.StartPlanting(parcel);
    }

    protected override void OnAllParcelsComplete()
    {
        state = OperatorState.Idle;
        movement.ClearTarget();
        Debug.Log($"[CropPlanter] Complete! {operation.TotalPlantsPlaced} plants, Cost: {operation.TotalCost:F2} EUR");
    }

    protected override void UpdateIdle() { }
    protected override string GetWorkingStatus() => $"Planting {operation?.PlantIndex}/{operation?.TotalPositions}";
    protected override string GetIdleStatus() => "Idle / Done";

    public bool IsPlanting => state == OperatorState.Working;
    public int PlantsPlaced => operation != null ? operation.TotalPlantsPlaced : 0;
    public float TotalSeedCost => operation != null ? operation.TotalCost : 0;
}