using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MakeMesh : MonoBehaviour
{
    private Mesh mesh;
    public static List<int> tris = new List<int>();
    public static List<Vector3> verts = new List<Vector3>();
    public static List<Vector3> normals = new List<Vector3>();
    private List<Color> colors = new List<Color>();
    private List<Vector2> uvs = new List<Vector2>();
    bool GO = false;
    int d = 0;
    public List<Material> materials = new List<Material>();
    int currentMaterial = 0;


    private Color Hidden = new Color(0f, 0f, 0f, 0f);

    public void ClearAll()
    {
        GO = false;
        if (mesh != null)
            mesh.Clear();

        tris.Clear();
        verts.Clear();
        normals.Clear();
        colors.Clear();
        uvs.Clear();
        d = 0;
    }
    public void Begin()
    {
        gameObject.transform.position = Vector3.zero;
        camScript.MinBone = Vector3.one * 1000;
        camScript.MaxBone = Vector3.one * -1000;
        camScript.MinSocket = Vector3.one * 1000;
        camScript.MaxSocket = Vector3.one * -1000;
        if (gameObject.GetComponent<MeshFilter>() == null)
            gameObject.AddComponent<MeshFilter>();
        if (gameObject.GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();

            mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        var rnd = GetComponent<MeshRenderer>();
        rnd.material = material;
        rnd.receiveShadows = true;
        rnd.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        GO = true;
    }


    public Material material { get; set;}
    public Color color { get; set; }

    public Vector3 Normal (Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var dir = Vector3.Cross(p2 - p1, p3 - p1);
        var norm = Vector3.Normalize(dir);
        return norm;
    }
    
    public void AddTriangle (Vector3 p1, Vector3 p2, Vector3 p3, Vector3 norm, bool _binary)
    {
        var count = tris.Count;

        tris.Add(count);
        tris.Add(count + 1);
        tris.Add(count + 2);
        if (_binary)
        {
            verts.Add(p3);
            verts.Add(p2);
            verts.Add(p1);
            uvs.Add(p3);
            uvs.Add(p2);
            uvs.Add(p1);
        }
        else
        {
            verts.Add(p1);
            verts.Add(p2);
            verts.Add(p3);
            uvs.Add(p1);
            uvs.Add(p2);
            uvs.Add(p3);
        }
        for (int i = 0; i < 3; i++)
            normals.Add(norm);
        for (int i = 0; i < 3; i++)
            colors.Add(camScript.stlColor);
    }

    public void MergeMesh()
    {
        var count = verts.Count;        
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();        
    }

    public Vector3[] GetTriangleVertices(int id)
    {
        var pts = new Vector3[3];
        var index = id * 3;
        pts[0] = mesh.vertices[index];
        pts[1] = mesh.vertices[index + 1];
        pts[2] = mesh.vertices[index + 2];
        return pts;
    }

    public Mesh GetMesh()
    {
        return mesh;
    }

    void Update()
    {        
    }

}
