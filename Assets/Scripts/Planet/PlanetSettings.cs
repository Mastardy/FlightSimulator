using UnityEngine;

[CreateAssetMenu()]
public class PlanetSettings : ScriptableObject
{
    [Header("Shape")]
    #region Shape
    
    public float planetRadius = 1;
    
    #endregion
    
    [Space(2), Header("Color")]
    #region Color
    
    public Material planetMaterial;
    public BiomeColorSettings biomeColorSettings;
    public Gradient oceanColor;

    [System.Serializable]
    public class BiomeColorSettings
    {
        public Biome[] biomes;
        public NoiseSettings noise;
        public float noiseOffset;
        public float noiseStrength;

        [Range(0, 1)] public float blendAmount;
        
        [System.Serializable]
        public class Biome
        {
            public Gradient gradient;
            public Color tint;
            [Range(0, 1)] public float startHeight;
            [Range(0, 1)] public float tintPercent;
        }
    }
    
    #endregion
    
    [Space(2), Header("Noise")]
    #region Noise
    
    public NoiseLayer[] noiseLayers;

    #endregion
}
