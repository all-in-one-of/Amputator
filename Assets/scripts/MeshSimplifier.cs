using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshSimplifier
{
    List<Triangle> tris = new List<Triangle>();
    float minTriangleSize;
    private List<SliceGlLine> lines = new List<SliceGlLine>();
    bool isBone = true;

    public MeshSimplifier(bool _isBone)
    {
        if (_isBone)
            tris = camScript.triangleListBone;
        else
            tris = camScript.triangleListSocket;
        isBone = _isBone;
        minTriangleSize = Camera.main.GetComponent<camScript>().MinTriangleSize;
        Simplify();
    }

    void Simplify()
    {
        var removeList = new List<int>();
        foreach(var tri in tris)
        {
            var p1 = tri.p1;
            var p2 = tri.p2;
            var p3 = tri.p3;
            //// 12 is base
            //var m_31 = Vector3.Normalize(p1 - p3);
            //var n_31 = Vector3.Normalize(new Vector3(-1f / m_31.x, -1f / m_31.y, -1f / m_31.z));
            //Ray ray = new Ray(p3, m_31);
            //var h = Vector3.Cross(ray.direction, p2 - ray.origin).magnitude;
            //var a = (Vector3.Distance(p3, p1) * h) / 2;
            var a = Vector3.Distance(p1, p2) + Vector3.Distance(p2, p3) + Vector3.Distance(p3, p1);
            if (a < minTriangleSize)
            {
                removeList.Add(tris.IndexOf(tri));
            }
        }
        RemoveTriangles(removeList);
        camScript.triangleListBone = tris;
    }

    void RemoveTriangles(List<int> _triIndices)
    {
        var dic = new Dictionary<int, Triangle>();
        foreach (var tri in tris)
        {
            dic.Add(tris.IndexOf(tri), tri);
        }
        foreach (var triIndex in _triIndices)
        {
            var triToRemove = dic[triIndex];
            var p1 = triToRemove.p1;
            var p2 = triToRemove.p2;
            var p3 = triToRemove.p3;
            var min = Vector3.one * 100000;
            var max = Vector3.one * -100000;
            if (p1.x < min.x) min.x = p1.x;
            if (p1.x > max.x) max.x = p1.x;
            if (p1.y < min.y) min.y = p1.y;
            if (p1.y > max.y) max.y = p1.y;
            if (p1.z < min.z) min.z = p1.z;
            if (p1.z > max.z) max.z = p1.z;
            var c = (min + max) / 2;
            for (int i = 0; i < tris.Count; i++)
            {
                if (triIndex == i || !dic.ContainsKey(i)) continue;
                if (triToRemove.p1 == dic[i].p1) dic[i].p1 = c;
                if (triToRemove.p1 == dic[i].p2) dic[i].p2 = c;
                if (triToRemove.p1 == dic[i].p3) dic[i].p3 = c;

                if (triToRemove.p2 == dic[i].p1) dic[i].p1 = c;
                if (triToRemove.p2 == dic[i].p2) dic[i].p2 = c;
                if (triToRemove.p2 == dic[i].p3) dic[i].p3 = c;

                if (triToRemove.p3 == dic[i].p1) dic[i].p1 = c;
                if (triToRemove.p3 == dic[i].p2) dic[i].p2 = c;
                if (triToRemove.p3 == dic[i].p3) dic[i].p3 = c;
            }
            dic.Remove(triIndex);
        }
        tris.Clear();
        foreach (var tri in dic)
        {
            tris.Add(tri.Value);
        }
    }
}
