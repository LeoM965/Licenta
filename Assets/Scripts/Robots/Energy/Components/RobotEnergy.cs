using UnityEngine;
using UnityEngine.Events;
using Robots.Models;
using Economics.Managers;

public class RobotEnergy : MonoBehaviour
{
    [SerializeField] private RobotBattery battery = new RobotBattery();

    public UnityEvent<float> OnBatteryChanged;
    public UnityEvent OnBatteryCritical;

    private Vector3 lastPosition;
    private bool isWorking;
    private bool isCharging;

    public float BatteryPercent => battery.Percentage;
    public float CurrentBattery => battery.currentKWh;

    private void Start()
    {
        lastPosition = transform.position;
        
        var data = RobotDataLoader.FindByName(name);
        if (data != null)
        {
            battery.maxKWh = data.batteryCapacity / 1000f;
            battery.currentKWh = battery.maxKWh;
            battery.consumptionMeter = data.consumptionMeter;
            battery.consumptionWorkSec = data.consumptionWorkSec;
            battery.consumptionStandbySec = data.consumptionStandbySec;
        }
        OnBatteryChanged?.Invoke(BatteryPercent);
    }

    private void Update()
    {
        if (isCharging)
        {
            battery.Recharge(Time.deltaTime);
            if (battery.IsFull) isCharging = false;
            OnBatteryChanged?.Invoke(BatteryPercent);
            return;
        }

        float consumed = 0f;
        float dist = Vector3.Distance(transform.position, lastPosition);
        bool isActive = dist > 0.001f || isWorking;

        if (dist > 0.001f)
        {
            consumed += dist * battery.consumptionMeter;
            lastPosition = transform.position;
            
            if (TimeManager.Instance != null)
                TimeManager.Instance.AddDistanceTraveled(dist);
        }

        float multiplier = SimulationSpeedController.Instance != null ? SimulationSpeedController.Instance.FairnessMultiplier : 1f;
        float effectiveDT = Time.deltaTime * multiplier;

        if (isWorking)
            consumed += battery.consumptionWorkSec * effectiveDT;
        else
            consumed += battery.consumptionStandbySec * effectiveDT;

        if (consumed > 0)
        {
            Consume(consumed);
            
            // Raportam la Economics DOAR daca robotul se misca sau lucreaza
            if (isActive && RobotEconomicsManager.Instance != null)
            {
                float simHoursDelta = effectiveDT / 3600f; // Fair delta based on amplified real-time
                RobotEconomicsManager.Instance.RecordStatus(transform, consumed, dist, simHoursDelta);
            }
        }
    }

    public void Consume(float amount)
    {
        battery.Consume(amount);
        OnBatteryChanged?.Invoke(BatteryPercent);
        if (battery.IsCritical) OnBatteryCritical?.Invoke();
    }

    public void StartCharging() => isCharging = true;
    public void SetWorking(bool working) => isWorking = working;

    public bool HasEnoughEnergy(float estimatedDistance, float estimatedWorkSeconds = 0f)
    {
        return battery.CanPerformTask(estimatedDistance, estimatedWorkSeconds);
    }

    public void SetFullBattery()
    {
        battery.currentKWh = battery.maxKWh;
        OnBatteryChanged?.Invoke(BatteryPercent);
    }
}
