using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)] 
    [SerializeField] private int resolution = 10;
    public int Resolution => resolution;

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
    private MeshFilter[] meshFilters;
    private TerrainFace[] terrainFaces;

    private void OnValidate()
    {
        GeneratePlanet();
    }
    
    private void Initialize()
    {
        shapeGenerator = new ShapeGenerator(settings);
        colorGenerator.UpdateSettings(settings);
        
        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

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
            if (meshFilters[i] == null)
            {
                var meshObject = new GameObject("PlanetMesh")
                {
                    transform =
                    {
                        parent = transform
                    }
                };

                meshObject.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = settings.planetMaterial;
            
            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
            var renderFace = faceRenderMask == FaceRenderMask.All || (int) faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }
    
    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void OnSettingsUpdated()
    {
        if (!autoUpdate) return;
        GeneratePlanet();   
    }
    
    private void GenerateMesh()
    {
        for (var i = 0; i < 6; i++)
        {
            if(meshFilters[i].gameObject.activeSelf) terrainFaces[i].ConstructMesh();
        }

        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    private void GenerateColors()
    {
        colorGenerator.UpdateColors();
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colorGenerator);
            }
        }
    }
}