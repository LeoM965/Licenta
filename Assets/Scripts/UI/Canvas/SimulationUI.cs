using UnityEngine;
using TMPro;
using Weather.Components;

namespace UI.Canvas
{
    public class SimulationUI : MonoBehaviour
    {
        public TextMeshProUGUI timeText;
        public TextMeshProUGUI seasonText;
        public TextMeshProUGUI weatherText;
        public TextMeshProUGUI tempText;

        private float timer;

        void Update()
        {
            if ((timer -= Time.deltaTime) <= 0f)
            {
                Refresh();
                timer = 1.0f; 
            }
        }

        private void Refresh()
        {
            if (TimeManager.Instance != null)
            {
                if (timeText) timeText.text = TimeManager.Instance.CurrentDate.ToString("HH:mm");
                if (seasonText) seasonText.text = GetSeasonRomanian(TimeManager.Instance.GetCurrentSeason());
            }

            if (WeatherSystem.Instance != null)
            {
                if (weatherText) weatherText.text = GetWeatherRomanian(WeatherSystem.Instance.CurrentWeather);
                if (tempText) tempText.text = $"{WeatherSystem.Instance.CurrentTemperature:F1}°C";
            }
        }

        private string GetSeasonRomanian(Weather.Models.Season s) => s switch
        {
            Weather.Models.Season.Spring => "Primăvară",
            Weather.Models.Season.Summer => "Vară",
            Weather.Models.Season.Autumn => "Toamnă",
            Weather.Models.Season.Winter => "Iarnă",
            _ => "N/A"
        };

        private string GetWeatherRomanian(Weather.Models.WeatherType t) => t switch
        {
            Weather.Models.WeatherType.Sunny => "Însorit",
            Weather.Models.WeatherType.Rainy => "Ploaie",
            Weather.Models.WeatherType.Stormy => "Furtună",
            Weather.Models.WeatherType.Foggy => "Ceață",
            Weather.Models.WeatherType.Snowy => "Zăpadă",
            _ => t.ToString()
        };
    }
}
