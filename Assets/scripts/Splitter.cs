using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Splitter
{
    public Splitter()
    {
        var y = Camera.main.GetComponent<camScript>().slicePlane.transform.position.y;
        var tris = camScript.triangleList;
        var gatherList = new List<Triangle>();
        foreach (var tri in tris)
        {
            var p1 = tri.p1;
            var p2 = tri.p2;
            var p3 = tri.p3;
            var norm = tri.norm;
            var _binary = tri._binary;

            var m_12 = Vector3.Normalize(p2 - p1);
            var m_23 = Vector3.Normalize(p3 - p2);
            var m_31 = Vector3.Normalize(p1 - p3);

            var n_12 = (y - p1.y) / m_12.y;
            var n_23 = (y - p2.y) / m_23.y;
            var n_31 = (y - p3.y) / m_31.y;

            if (p1.y < y && p2.y < y && p3.y < y) continue;        // All points below slice plane
            if (p1.y > y && p2.y > y && p3.y > y)                // All points above slice plane
            {
                gatherList.Add(tri);
                continue;
            }
            if (p1.y > y)                                       // p1 visible
            {
                if (p2.y < y && p3.y < y)                       //p2 and p3 are not visible. triangle
                {
                    p2 = p1 - n_12 * m_12;
                    p3 = p3 + n_31 * m_31;
                    var newTri = new Triangle(p1, p2, p3, norm, _binary);
                    gatherList.Add(newTri);
                }
                else if (p2.y < y)                              // Just p2 not visible. trapezoid
                {
                    var p12 = p1 + n_12 * m_12;
                    var p23 = p2 + n_23 * m_23;
                    var tri_1 = new Triangle(p1, p12, p3, norm, _binary);
                    var tri_2 = new Triangle(p12, p23, p3, norm, _binary);
                    gatherList.Add(tri_1);
                    gatherList.Add(tri_2);
                }
                else if (p3.y < y)                              // Just p3 not visible. trapezoid
                {
                    var p23 = p2 + n_23 * m_23;
                    var p31 = p3 + n_31 * m_31;
                    var tri_1 = new Triangle(p2, p23, p1, norm, _binary);
                    var tri_2 = new Triangle(p23, p31, p1, norm, _binary);
                    gatherList.Add(tri_1);
                    gatherList.Add(tri_2);
                }
            }

            else if (p2.y > y)                                       // p2 visible, p1 is not
            {

                if (p3.y > y)                              // p2 and p3 visible. p1 is not.
                {
                    var p12 = p1 + n_12 * m_12;
                    var p31 = p3 + n_31 * m_31;
                    var tri_1 = new Triangle(p3, p31, p2, norm, _binary);
                    var tri_2 = new Triangle(p31, p12, p2, norm, _binary);
                    gatherList.Add(tri_1);
                    gatherList.Add(tri_2);
                }
                else                                    // p2 is visible. p1 and p3 are not.
                {
                    var p12 = p1 + n_12 * m_12;
                    var p23 = p2 + n_23 * m_23;
                    var newTri = new Triangle(p12, p2, p23, norm, _binary);
                    gatherList.Add(newTri);
                }
            }
            else if (p3.y > y)                                       // p3 visible, p1 and p2 are not.
            {
                var p23 = p2 + n_23 * m_23;
                var p31 = p3 + n_31 * m_31;
                var newTri = new Triangle(p31, p23, p3, norm, _binary);
                gatherList.Add(newTri);
            }
        }
        camScript.tempTriangleList.Clear();
        camScript.tempTriangleList = gatherList;
        Camera.main.GetComponent<camScript>().Redraw();
    }
}
