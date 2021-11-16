using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* https://codereview.stackexchange.com/questions/219806/making-a-icosphere-in-c-and-unity */
/* https://github.com/codatproduction/Procedural-Low-Poly-Trees/blob/master/src/Leave.gd */

public class ProcGenBoulder : MonoBehaviour
{
    public int nDivision = 1;
    public float randomMagnitude = 1;
    private List<Vector3> verticeList;
    private List<Vector3> finalVerticeList = new List<Vector3>();
    private List<Triangle> triangleList;
    private int[] triangles;
    private List<Vector3> normalsList = new List<Vector3>();
    private MeshFilter meshFilter;
    private Mesh mesh;

    public void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        ReGenerate();
    }

    /// <summary>
    /// Re-generate the mesh
    /// </summary>
    public void ReGenerate()
    {
        // Create mesh here
        mesh = new Mesh();
        CreateIcosphere();
        SubDivision();
        GenerateMesh();

        // Assign the mesh to its Component
        meshFilter.mesh = mesh;
    }

    /// <summary>
    /// Create an icosphere, a sphere-like with 12 verticeList and 20 triangles.
    /// </summary>
    private void CreateIcosphere()
    {
        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        // Create verticeList
        verticeList = new List<Vector3>
        {
            new Vector3(-1, t, 0).normalized,
            new Vector3(1, t, 0).normalized,
            new Vector3(-1, -t, 0).normalized,
            new Vector3(1, -t, 0).normalized,
            new Vector3(0, -1, t).normalized,
            new Vector3(0, 1, t).normalized,
            new Vector3(0, -1, -t).normalized,
            new Vector3(0, 1, -t).normalized,
            new Vector3(t, 0, -1).normalized,
            new Vector3(t, 0, 1).normalized,
            new Vector3(-t, 0, -1).normalized,
            new Vector3(-t, 0, 1).normalized
        };

        // Assign them to triangles. These integers are id of vertice
        //from the array verticeList. Each line (3 number) form a triangle.
        triangleList = new List<Triangle>
        {
            new Triangle(0, 11, 5),
            new Triangle(0, 5, 1),
            new Triangle(0, 1, 7),
            new Triangle(0, 7, 10),
            new Triangle(0, 10, 11),
            new Triangle(1, 5, 9),
            new Triangle(5, 11, 4),
            new Triangle(11, 10, 2),
            new Triangle(10, 7, 6),
            new Triangle(7, 1, 8),
            new Triangle(3, 9, 4),
            new Triangle(3, 4, 2),
            new Triangle(3, 2, 6),
            new Triangle(3, 6, 8),
            new Triangle(3, 8, 9),
            new Triangle(4, 9, 5),
            new Triangle(2, 4, 11),
            new Triangle(6, 2, 10),
            new Triangle(8, 6, 7),
            new Triangle(9, 8, 1)
        };
    }

    /// <summary>
    /// Subdivison the icosphere to make it smoother, and has more verticeList.
    /// </summary>
    private void SubDivision()
    {
        var midPointCache = new Dictionary<int, int>();
        for (int i = 0; i < nDivision - 1; i++)
        {
            List<Triangle> newTriangleList = new List<Triangle>();

            foreach (Triangle triangle in triangleList)
            {
                int a = triangle.a;
                int b = triangle.b;
                int c = triangle.c;

                // Find the vertice at the center between pair of vertice
                int ab = GetMidPointIndex(midPointCache, a, b);
                int bc = GetMidPointIndex(midPointCache, b, c);
                int ca = GetMidPointIndex(midPointCache, c, a);

                newTriangleList.Add(new Triangle(a, ab, ca));
                newTriangleList.Add(new Triangle(b, bc, ab));
                newTriangleList.Add(new Triangle(c, ca, bc));
                newTriangleList.Add(new Triangle(ab, bc, ca));
            }
            triangleList = newTriangleList;
        }
    }

    /// <summary>
    /// Generate the actual mesh
    /// </summary>
    private void GenerateMesh()
    {
        List<Vector3> noiseVerticeList = new List<Vector3>();

        // Randomize vertices so it look organic
        foreach (Vector3 vertex in verticeList)
        {
            Vector3 normal = vertex.normalized;
            float randomNumber = Random.Range(0, 100);
            float noiseValue = Mathf.PerlinNoise(normal.x * randomNumber, normal.y * randomNumber);
            Vector3 newVertex = vertex + (noiseValue * randomMagnitude * normal);
            noiseVerticeList.Add(newVertex);
        }

        // Dupe the vertices to triple so we can have flat shading.
        // Every vertex on a triangle should have same normal
        foreach (Triangle triangle in triangleList)
        {
            Vector3 v1 = noiseVerticeList[triangle.a];
            Vector3 v2 = noiseVerticeList[triangle.b];
            Vector3 v3 = noiseVerticeList[triangle.c];

            Vector3 triangleNormal = Vector3.Cross((v2 - v1), (v3 - v1));
            finalVerticeList.Add(v1);
            finalVerticeList.Add(v2);
            finalVerticeList.Add(v3);
            normalsList.Add(triangleNormal);
            normalsList.Add(triangleNormal);
            normalsList.Add(triangleNormal);
        }

        triangles = new int[finalVerticeList.Count];
        for (int i = 0; i < finalVerticeList.Count; i++)
        {
            triangles[i] = i;
        }

        //int c = 0;
        //foreach (Triangle triangle in triangleList)
        //{
        //    triangles[c + 0] = triangle.a;
        //    triangles[c + 1] = triangle.b;
        //    triangles[c + 2] = triangle.c;
        //    c += 3;
        //}

        // Make the normals array
        //Vector3[] normals = new Vector3[noiseVerticeList.Count];
        //for (int i = 0; i < verticeList.Count; i++)
        //{
        //    normals[i] = verticeList[i].normalized;
        //}

        // Assign things to the mesh
        mesh.name = "LowPolyLeave";
        mesh.vertices = finalVerticeList.ToArray();
        mesh.triangles = triangles;
        mesh.normals = normalsList.ToArray();
    }

    public int GetMidPointIndex(Dictionary<int, int> cache, int indexA, int indexB)
    {
        // Checks if vertice has already been made and creates it if it hasn't
        int smallerIndex = Mathf.Min(indexA, indexB);
        int greaterIndex = Mathf.Max(indexA, indexB);
        int key = (smallerIndex << 16) + greaterIndex;

        if (cache.TryGetValue(key, out int ret))
            return ret;

        Vector3 p1 = verticeList[indexA];
        Vector3 p2 = verticeList[indexB];
        Vector3 middle = Vector3.Lerp(p1, p2, 0.5f).normalized;

        ret = verticeList.Count;

        verticeList.Add(middle);

        cache.Add(key, ret);
        return ret;
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            Gizmos.DrawWireSphere(transform.position, 1);
    }
}

// Support class for storing group of 3 verticeList id
public class Triangle
{
    public int a, b, c;

    public Triangle(int a, int b, int c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    public string TriangleToString()
    {
        return a + "," + b + "," + c;
    }
}