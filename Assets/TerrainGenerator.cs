using UnityEngine;
using UnityEngine.UI;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 100;  // Number of squares/vertices along the x-axis
    public int depth = 100;  // Number of squares/vertices along the z-axis    
    public float scale = 5f; // Scale of the perlin noise

    public Slider hillinessSlider;
    public Button regenerateButton;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
  
        hillinessSlider.onValueChanged.AddListener(UpdateHilliness);
        regenerateButton.onClick.AddListener(GenerateAndApplyTerrain);

        UpdateHilliness(hillinessSlider.value);
        GenerateAndApplyTerrain();
    }

    void UpdateHilliness(float newScale)
    {
        scale = Mathf.Lerp(0f, 15f, newScale); ;
    }

    void GenerateAndApplyTerrain()
    {
        GenerateTerrain();
        UpdateMesh();
    }

    void GenerateTerrain()
    {
        vertices = new Vector3[(width + 1) * (depth + 1)];
        int i = 0;

        for (int z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = Mathf.PerlinNoise(x * 0.1f, z * 0.1f) * scale;

                y = Mathf.Floor(y * 10) / 10f;// for avoiding Uneven vertex height distribution 

                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[width * depth * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;

                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            Destroy(meshCollider);
        }
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
}
