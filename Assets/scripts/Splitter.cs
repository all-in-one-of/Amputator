using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Splitter
{
    private float sliceHeight = 0;

    public Splitter()
    {
        sliceHeight = Camera.main.GetComponent<camScript>().slicePlane.transform.position.y;
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

            var n_12 = (sliceHeight - p1.y) / m_12.y;
            var n_23 = (sliceHeight - p2.y) / m_23.y;
            var n_31 = (sliceHeight - p3.y) / m_31.y;

            if (p1.y < sliceHeight && p2.y < sliceHeight && p3.y < sliceHeight) continue;        // All points below slice plane
            if (p1.y > sliceHeight && p2.y > sliceHeight && p3.y > sliceHeight)                // All points above slice plane
            {
                gatherList.Add(tri);
                continue;
            }
            if (p1.y > sliceHeight)                                       // p1 visible
            {
                if (p2.y < sliceHeight && p3.y < sliceHeight)                       //p2 and p3 are not visible. triangle
                {
                    p2 = p1 + n_12 * m_12;
                    p3 = p3 + n_31 * m_31;
                    var newTri = new Triangle(p1, p2, p3, norm, _binary);
                    gatherList.Add(newTri);
                }
                else if (p2.y < sliceHeight)                              // Just p2 not visible. trapezoid
                {
                    var p12 = p1 + n_12 * m_12;
                    var p23 = p2 + n_23 * m_23;
                    var tri_1 = new Triangle(p3, p1, p12, norm, _binary);
                    var tri_2 = new Triangle(p12, p23, p3, norm, _binary);
                    gatherList.Add(tri_1);
                    gatherList.Add(tri_2);
                }
                else if (p3.y < sliceHeight)                              // Just p3 not visible. trapezoid
                {
                    var p23 = p2 + n_23 * m_23;
                    var p31 = p3 + n_31 * m_31;
                    var tri_1 = new Triangle(p1, p2, p23, norm, _binary);
                    var tri_2 = new Triangle(p23, p31, p1, norm, _binary);
                    gatherList.Add(tri_1);
                    gatherList.Add(tri_2);
                }
            }

            else if (p2.y > sliceHeight)                                       // p2 visible, p1 is not
            {

                if (p3.y > sliceHeight)                              // p2 and p3 visible. p1 is not.
                {
                    var p12 = p1 + n_12 * m_12;
                    var p31 = p3 + n_31 * m_31;
                    var tri_1 = new Triangle(p2, p3, p31, norm, _binary);
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
            else if (p3.y > sliceHeight)                                       // p3 visible, p1 and p2 are not.
            {
                var p23 = p2 + n_23 * m_23;
                var p31 = p3 + n_31 * m_31;
                var newTri = new Triangle(p23, p3, p31, norm, _binary);
                gatherList.Add(newTri);
            }
        }
        camScript.tempTriangleList.Clear();
        camScript.tempTriangleList = gatherList;
        camScript.tempTriangleList.AddRange(CapIt(gatherList));
        Camera.main.GetComponent<camScript>().Redraw();
    }

    public List<Triangle> CapIt(List<Triangle> gatherList)
    {
        var Max = Vector3.one * -100000;
        var Min = Vector3.one * 100000;
        var centr = Vector3.zero;
        if (sliceHeight > -1000)
        {
            centr.y = sliceHeight;
        }
        var capTris = new List<Triangle>();
        foreach (var tri in gatherList)
        {
            var p1 = tri.p1;
            var p2 = tri.p2;
            var p3 = tri.p3;
            var norm = Vector3.down;// tri.norm;    // all normals point straight down.
            var _binary = tri._binary;

            if (p1.y != sliceHeight && p2.y != sliceHeight && p3.y != sliceHeight) continue;

            #region diff
            if (p1.y == sliceHeight)                        // p1 on plane. p2 and p3 could be either.
            {
                if (p2.y == sliceHeight)                  // p1 and p2 on plane. p3 can not be on plane.
                {
                    capTris.Add(new Triangle(p2, p1, centr, norm, _binary));
                }
                else if (p3.y == sliceHeight)             // p1 and p3 on plane. p2 can not be on plane.
                {
                    capTris.Add(new Triangle(p1, p3, centr, norm, _binary));
                }
                else                            // Just p1 on plane. p2 and p3 are not.
                {

                }

            }
            else if (p2.y == sliceHeight)                 // p2 on plane. p1 is not. p3 could be either.
            {
                if (p3.y == sliceHeight)                   // p2 and p3 on plane. p1 is not.
                {
                    capTris.Add(new Triangle(p3, p2, centr, norm, _binary));
                }
                else                            // Just p2 on plane. p1 and p3 are not.
                {

                }

            }
            else                                // Just p3 on plane. p1 and p2 are not.
            {

            }
            #endregion
        }
        if (sliceHeight > -1000)
        {
            Min.y = sliceHeight;
        }
        foreach (var capTri in capTris)
        {
            if (capTri.p1.x > Max.x) Max.x = capTri.p1.x;
            if (capTri.p1.z > Max.z) Max.z = capTri.p1.z;
            if (capTri.p1.x < Min.x) Min.x = capTri.p1.x;
            if (capTri.p1.z < Min.z) Min.z = capTri.p1.z;

            if (capTri.p2.x > Max.x) Max.x = capTri.p2.x;
            if (capTri.p2.z > Max.z) Max.z = capTri.p2.z;
            if (capTri.p2.x < Min.x) Min.x = capTri.p2.x;
            if (capTri.p2.z < Min.z) Min.z = capTri.p2.z;
            
        }
        centr = (Max + Min) / 2;
        centr.y = sliceHeight;
        foreach (var capTri in capTris)
        {
            capTri.p3 = centr;
        }
        return capTris; 
    }
}
