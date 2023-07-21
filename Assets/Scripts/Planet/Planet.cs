using System;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)] 
    [SerializeField] private int resolution = 10;
    public int Resolution => resolution;

    [Range(1, 5)] [SerializeField] private int faceResolution = 2;

    public enum FaceRenderMask
    {
        All,
        Top,
        Bottom,
        Left,
        Right,
        Front,
        Back
    }
    public FaceRenderMask faceRenderMask;

    public bool autoUpdate = true;
    
    public PlanetSettings settings;

    private ShapeGenerator shapeGenerator;
    private ColorGenerator colorGenerator = new();

    [HideInInspector]
    public bool settingsFoldout;
    
    [SerializeField, HideInInspector]
    private MeshFilter[,] meshFilters;
    private TerrainFace[,] terrainFaces;

    private void Initialize()
    {
        shapeGenerator = new ShapeGenerator(settings);
        colorGenerator.UpdateSettings(settings);
        
        if (meshFilters == null || meshFilters.Length == 0 || meshFilters.Length != 6 * faceResolution * faceResolution)
        {
            meshFilters = new MeshFilter[6, faceResolution * faceResolution];
        }
        terrainFaces = new TerrainFace[6, faceResolution * faceResolution];

        Vector3[] directions =
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right,
            Vector3.forward,
            Vector3.back
        };
        
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < faceResolution * faceResolution; j++)
            {
                if (meshFilters[i, j] == null)
                {
                    var meshObject = new GameObject("PlanetMesh")
                    {
                        transform =
                        {
                            parent = transform
                        }
                    };

                    meshObject.AddComponent<MeshRenderer>();
                    meshFilters[i, j] = meshObject.AddComponent<MeshFilter>();
                    meshFilters[i, j].sharedMesh = new Mesh();
                }

                meshFilters[i, j].GetComponent<MeshRenderer>().sharedMaterial = settings.planetMaterial;

                terrainFaces[i, j] = new TerrainFace(shapeGenerator, meshFilters[i, j].sharedMesh, resolution, faceResolution, j, directions[i]);
                var renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
                meshFilters[i, j].gameObject.SetActive(renderFace);
            }
        }
    }
    
    public void GeneratePlanet()
    {
        var children = GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            var child = children[i].gameObject;
            if (child == gameObject) continue;
            DestroyImmediate(child);
        }

        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void OnSettingsUpdated()
    {
        if (!autoUpdate) return;
        GeneratePlanet();   
    }

    private void Awake()
    {
        GeneratePlanet();
    }

    private void GenerateMesh()
    {
        for (var i = 0; i < 6; i++)
        {
            for (int j = 0; j < faceResolution * faceResolution; j++)
            {
                if(meshFilters[i, j].gameObject.activeSelf) terrainFaces[i, j].ConstructMesh();
            }
        }

        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    private void GenerateColors()
    {
        colorGenerator.UpdateColors();
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < faceResolution * faceResolution; j++)
            {
                if (meshFilters[i, j].gameObject.activeSelf)
                {
                    terrainFaces[i, j].UpdateUVs(colorGenerator);
                }
            }
        }
    }
}