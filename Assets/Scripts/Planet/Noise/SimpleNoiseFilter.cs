using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter
{
    private NoiseSettings.SimpleNoiseSettings settings;
    private Noise noise = new();

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
    {
        this.settings = settings;
    }
    
    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.noiseBaseRoughness;
        float amplitude = 1;

        for (int i = 0; i < settings.noiseLayersAmount; i++)
        {
            float v = noise.Evaluate(point * frequency + settings.noiseCenter);
            noiseValue += (v + 1) * 0.5f * amplitude;
            frequency *= settings.noiseRoughness;
            amplitude *= settings.noisePersistence;
        }
        
        noiseValue = noiseValue - settings.noiseMinValue;
        return noiseValue * settings.noiseStrength;
    }
}
