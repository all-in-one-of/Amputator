using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshSimplifier
{
    List<Triangle> tris = new List<Triangle>();
    float precision;
    private List<SliceGlLine> lines = new List<SliceGlLine>();
    bool isBone = true;

    public MeshSimplifier(bool _isBone)
    {
        if (_isBone)
            tris = camScript.triangleListBone;
        else
            tris = camScript.triangleListSocket;
        isBone = _isBone;
        precision = Camera.main.GetComponent<camScript>().Precision;
        lines = new List<SliceGlLine>();
    }
}
