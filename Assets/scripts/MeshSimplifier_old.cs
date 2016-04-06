using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshSimplifier_old
{
    List<Triangle> tris = new List<Triangle>();
    float precision;

    public MeshSimplifier_old(bool _isBone)
    {
        if (_isBone)
            tris = camScript.triangleListBone;
        else
            tris = camScript.triangleListSocket;
        precision = Camera.main.GetComponent<camScript>().Precision;
        var verts = new List<VertexToSimplify>();
        foreach (var tri in tris)
        {
            var ind = tris.IndexOf(tri);
            verts.Add(new VertexToSimplify(tri.p1, ind, 1));
            verts.Add(new VertexToSimplify(tri.p2, ind, 2));
            verts.Add(new VertexToSimplify(tri.p3, ind, 3));
        }
        for (int i = 0; i < verts.Count; i++)
        {
            var v = verts[i].Vertex;
            for (int j = i + 1; j < verts.Count; j++)
            {
                var v2 = verts[j].Vertex;
                if (Vector3.Distance(v, v2) < precision)
                {
                    verts[j].Vertex = v;
                }
            }
        }
        foreach (var v in verts)
        {
            switch (v.IndexInTriangle)
            {
                case 1:
                    tris[v.IndexInTrianglesList].p1 = v.Vertex;
                    break;
                case 2:
                    tris[v.IndexInTrianglesList].p2 = v.Vertex;
                    break;
                case 3:
                    tris[v.IndexInTrianglesList].p2 = v.Vertex;
                    break;
                default:
                    break;
            }
            tris[v.IndexInTrianglesList].GenerateNormal();
        }
        for (int i = 0; i < tris.Count; i++)
        {
            var tri = tris[i];
            for (int j = i + 1; j < tris.Count; j++)
            {
                var tri2 = tris[j];
                if (tri.p1 == tri2.p1)
                {
                    if ((tri.p2 == tri2.p2 && tri.p3 == tri2.p3) || (tri.p2 == tri2.p3 && tri.p3 == tri2.p2))
                        tris.RemoveAt(j);
                    continue;
                }
                else if (tri.p1 == tri2.p2)
                {
                    if ((tri.p2 == tri2.p1 && tri.p3 == tri2.p3) || (tri.p2 == tri2.p3 && tri.p3 == tri2.p1))
                        tris.RemoveAt(j);
                    continue;
                }
                else if (tri.p1 == tri2.p3)
                {
                    if ((tri.p2 == tri2.p1 && tri.p3 == tri2.p2) || (tri.p2 == tri2.p2 && tri.p3 == tri2.p1))
                        tris.RemoveAt(j);
                    continue;
                }
            }
        }
        if (_isBone)
            camScript.triangleListBone = tris;
        else
            camScript.triangleListSocket = tris;
    }


}

