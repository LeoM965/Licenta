using System;
using System.Linq;

namespace Weather.Models
{
    public enum WeatherType
    {
        Sunny,
        Rainy,
        Stormy,
        Snowy,
        Foggy
    }

    public static class WeatherTypes
    {
        public static readonly WeatherType[] All = (WeatherType[])Enum.GetValues(typeof(WeatherType));
    }
}
