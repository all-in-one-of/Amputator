using UnityEngine;
using System.Collections;

public class SliceGlLine
{
    public SliceGlLine (Vector3 _p1, Vector3 _p2)
    {
        p1 = _p1;
        p2 = _p2;
    }
    public Vector3 p1 { get; set; }
    public Vector3 p2 { get; set; }
}
