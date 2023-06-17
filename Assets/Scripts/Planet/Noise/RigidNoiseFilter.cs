using UnityEngine;

public class RigidNoiseFilter : INoiseFilter
{
    private NoiseSettings.RigidNoiseSettings settings;
    private Noise noise = new();

    public RigidNoiseFilter(NoiseSettings.RigidNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.noiseBaseRoughness;
        float amplitude = 1;
        float weight = 1;

        for (int i = 0; i < settings.noiseLayersAmount; i++)
        {
            float v = 1 - Mathf.Abs(noise.Evaluate(point * frequency + settings.noiseCenter));
            v *= v;
            v *= weight;
            weight = Mathf.Clamp01(v * settings.noiseWeightMultiplier);
            noiseValue += v * 0.5f * amplitude;
            frequency *= settings.noiseRoughness;
            amplitude *= settings.noisePersistence;
        }

        noiseValue = noiseValue - settings.noiseMinValue;
        return noiseValue * settings.noiseStrength;
    }
}