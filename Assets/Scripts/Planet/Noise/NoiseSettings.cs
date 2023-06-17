using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType
    {
        Simple,
        Rigid
    };
    public FilterType filterType;

    [ConditionalHide("filterType", 0)]
    public SimpleNoiseSettings simpleNoiseSettings;
    [ConditionalHide("filterType", 1)]
    public RigidNoiseSettings rigidNoiseSettings;
    
    [System.Serializable]
    public class SimpleNoiseSettings
    {
        [Range(1, 8)] public int noiseLayersAmount = 1;
        public float noiseStrength = 1;
        public float noiseBaseRoughness = 1;
        public float noiseRoughness = 2;
        public float noisePersistence = 0.5f;
        public Vector3 noiseCenter;
        public float noiseMinValue = 1;   
    }

    [System.Serializable]
    public class RigidNoiseSettings : SimpleNoiseSettings
    {
        public float noiseWeightMultiplier = 0.8f;   
    }
}