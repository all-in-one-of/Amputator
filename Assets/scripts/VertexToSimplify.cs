using UnityEngine;
using System.Collections;

public class VertexToSimplify
{ 
    public VertexToSimplify (Vector3 _vertex, int _indexInTrianglesList, int _indexInTriangle)
    {
        Vertex = _vertex;
        IndexInTrianglesList = _indexInTrianglesList;
        IndexInTriangle = _indexInTriangle;
    }
    public Vector3 Vertex { get; set; }
    public int IndexInTrianglesList { get; set; }
    public int IndexInTriangle { get; set; }
}
