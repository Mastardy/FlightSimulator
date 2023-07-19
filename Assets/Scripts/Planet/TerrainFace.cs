using UnityEngine;

public class TerrainFace
{
    private ShapeGenerator shapeGenerator;
    private Mesh mesh;
    private int resolution;
    private Vector3 localUp;
    private Vector3 axisA;
    private Vector3 axisB;

    private int faceResolution;
    private Vector2Int faceIndex;
    
    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, int faceResolution, int faceIndex, Vector3 localUp)
    {
        this.faceResolution = faceResolution;
        this.faceIndex = new Vector2Int(faceIndex % faceResolution, faceIndex / faceResolution);

        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triangleIndex = 0;
        Vector2[] uv = mesh.uv.Length == vertices.Length ? mesh.uv : new Vector2[vertices.Length];
        
        for(int y = 0, i = 0; y < resolution; y++)
        {
            for(int x = 0; x < resolution; x++, i++)
            {
                Vector2 percent = (new Vector2(x, y) / (resolution - 1) / faceResolution) + (Vector2)faceIndex / faceResolution;
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                float unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);
                vertices[i] = pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);
                uv[i].y = unscaledElevation;
                
                if(x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triangleIndex++] = i;
                    triangles[triangleIndex++] = i + resolution + 1;
                    triangles[triangleIndex++] = i + resolution;
                    triangles[triangleIndex++] = i;
                    triangles[triangleIndex++] = i + 1;
                    triangles[triangleIndex++] = i + resolution + 1;
                }
            }
        }
        
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
    }

    public void UpdateUVs(ColorGenerator colorGenerator)
    {
        Vector2[] uv = mesh.uv;
        
        for(int y = 0, i = 0; y < resolution; y++)
        {
            for(int x = 0; x < resolution; x++, i++)
            {
                Vector2 percent = new Vector2(x, y) / (resolution - 1) / faceResolution + (Vector2)faceIndex / faceResolution;
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                
                uv[i].x = colorGenerator.BiomePercentFromPoint(pointOnUnitSphere);
            }
        }

        mesh.uv = uv;
    }
}
