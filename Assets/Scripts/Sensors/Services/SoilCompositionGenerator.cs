using UnityEngine;
using Sensors.Models;

namespace Sensors.Services
{
    public static class SoilCompositionGenerator
    {
        public static SoilComposition Generate(AgroSoilType type, SoilSettings settings)
        {
            if (settings != null && settings.typeRanges != null)
            {
                foreach (var range in settings.typeRanges)
                {
                    if (range.type == type) return range.Generate();
                }
            }

            return new SoilComposition
            {
                moisture = 50f,
                pH = 6.5f,
                nitrogen = 350f,
                phosphorus = 20f,
                potassium = 220f
            };
        }
    }
}
