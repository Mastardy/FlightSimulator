using UnityEngine;

public class ColorGenerator
{
    private PlanetSettings settings;

    private Texture2D texture;
    const int textureResolution = 50;
    private INoiseFilter biomeNoiseFilter;
    
    private static readonly int minMaxVector = Shader.PropertyToID("_ElevationMinMax");
    private static readonly int planetTexture = Shader.PropertyToID("_PlanetTexture");

    public void UpdateSettings(PlanetSettings settings)
    {
        this.settings = settings;
        var biomeAmount = settings.biomeColorSettings.biomes.Length;
        if (texture == null || texture.height != biomeAmount) texture = new Texture2D(textureResolution * 2, biomeAmount, TextureFormat.RGBA32, false);
        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.planetMaterial.SetVector(minMaxVector, new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;

        heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - settings.biomeColorSettings.noiseOffset) * settings.biomeColorSettings.noiseStrength;
        
        float biomeIndex = 0;
        int numBiomes = settings.biomeColorSettings.biomes.Length;

        float blendRange = settings.biomeColorSettings.blendAmount / 2.0f + 0.001f;
        
        for (int i = 0; i < numBiomes; i++)
        {
            float distance = heightPercent - settings.biomeColorSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, distance);
            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;
        }
        
        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }
    
    public void UpdateColors()
    {
        Color[] colors = new Color[texture.width * texture.height];
        int colorIndex = 0;
        foreach(var biome in settings.biomeColorSettings.biomes)
        {
            for (int i = 0; i < textureResolution * 2; i++)
            {
                var gradientColor = i < textureResolution ? 
                    settings.oceanColor.Evaluate(i / (textureResolution - 1f)) : 
                    biome.gradient.Evaluate((i - textureResolution) / (textureResolution - 1f));
                
                Color tintColor = biome.tint;

                colors[colorIndex] = gradientColor * (1 - biome.tintPercent) + tintColor * biome.tintPercent;
                colorIndex++;
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        settings.planetMaterial.SetTexture(planetTexture, texture);
    }
}