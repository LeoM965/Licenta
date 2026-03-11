using UnityEngine;
using Sensors.Services;
using Weather.Services;

namespace Weather.Components
{
    [RequireComponent(typeof(WeatherSystem))]
    public class WeatherSoilUpdater : MonoBehaviour
    {
        private WeatherSystem weatherSystem;
        private float lastSimHours = -1f;

        private void Awake()
        {
            weatherSystem = GetComponent<WeatherSystem>();
        }

        private void Update()
        {
            if (weatherSystem == null || weatherSystem.ActiveClimate == null || TimeManager.Instance == null) return;

            float currentSimHours = TimeManager.Instance.TotalSimulatedHours;
            if (lastSimHours < 0f) 
            { 
                lastSimHours = currentSimHours; 
                return; 
            }

            float deltaHours = currentSimHours - lastSimHours;

            lastSimHours = currentSimHours;
            
            if (ParcelCache.HasInstance)
            {
                SoilMoistureService.UpdateMoisture(
                    ParcelCache.Parcels, 
                    weatherSystem.CurrentImpact, 
                    weatherSystem.ActiveClimate, 
                    deltaHours
                );
            }
        }
    }
}
