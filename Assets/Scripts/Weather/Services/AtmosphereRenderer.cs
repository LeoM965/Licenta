using UnityEngine;
using Weather.Models;

namespace Weather.Services
{
    public class AtmosphereRenderer
    {
        private Light sunLight;
        private float lerpSpeed = 1.0f;

        public AtmosphereRenderer(Light sun)
        {
            sunLight = sun;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
        }

        public void Render(WeatherProfile profile, float timeOfDay, float deltaTime)
        {
            if (profile == null) return;

            // Handle Skybox
            if (RenderSettings.skybox != profile.skyboxMaterial)
            {
                RenderSettings.skybox = profile.skyboxMaterial;
                DynamicGI.UpdateEnvironment();
            }

            // Time-based light modification (simple night/day)
            float nightFactor = 1.0f;
            if (timeOfDay < 5f || timeOfDay > 20f) nightFactor = 0.05f; // Night
            else if (timeOfDay < 7f) nightFactor = Mathf.InverseLerp(5f, 7f, timeOfDay); // Dawn
            else if (timeOfDay > 18f) nightFactor = Mathf.InverseLerp(20f, 18f, timeOfDay); // Dusk

            if (sunLight != null)
            {
                sunLight.color = Color.Lerp(sunLight.color, profile.sunColor, deltaTime * lerpSpeed);
                sunLight.intensity = Mathf.Lerp(sunLight.intensity, profile.sunIntensity * nightFactor, deltaTime * lerpSpeed);
            }

            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, profile.fogColor, deltaTime * lerpSpeed);
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, profile.fogDensity, deltaTime * lerpSpeed);
            RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, profile.ambientIntensity * nightFactor, deltaTime * lerpSpeed);
        }
    }
}
