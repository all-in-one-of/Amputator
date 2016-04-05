using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SliceDisplay : MonoBehaviour {

    public Material mat;
    public Color col;
    int j = 0;
    private List<SliceGlLine> lines = new List<SliceGlLine>();
	void OnGUI()
    {
        
        foreach (var l in lines)
        {
            GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
            GL.modelview = Camera.main.worldToCameraMatrix;
            GL.Begin(GL.LINES);
            mat.SetPass(0);
            GL.Color(col);
            GL.Vertex(l.p1);
            GL.Vertex(l.p2);
            GL.End();
        }
    }
	public void Update ()
    {
        lines.Clear();
        var y = Camera.main.GetComponent<camScript>().slicePlane.transform.position.y;
        foreach (var tri in camScript.triangleList)
        {
            var p1 = tri.p1;
            var p2 = tri.p2;
            var p3 = tri.p3;

            var m_12 = Vector3.Normalize(p2 - p1);
            var m_23 = Vector3.Normalize(p3 - p2);
            var m_31 = Vector3.Normalize(p1 - p3);

            var t_12 = (y - p1.y) / m_12.y;
            var t_23 = (y - p2.y) / m_23.y;
            var t_31 = (y - p3.y) / m_31.y;


            if (p1.y > y)                                       // p1 above. p2 and p3 unknown.
            {
                if (p2.y > y && p3.y > y) continue;             // All are above
                if (p2.y > y)                                   // p1 p2 above. p3 below.
                {
                    var p_23 = p2 + m_23 * t_23;
                    var p_31 = p3 + m_31 * t_31;
                    var s = new SliceGlLine(p_23, p_31);
                    lines.Add(s);
                }
                else if (p3.y > y)                              // p1 p3 above. p2 below.
                {
                    var p_12 = p1 + m_12 * t_12;
                    var p_23 = p2 + m_23 * t_23;
                    var s = new SliceGlLine(p_12, p_23);
                    lines.Add(s);
                }
                else                                            // p1 above. p2 p3 below.
                {
                    var p_12 = p1 + m_12 * t_12;
                    var p_31 = p3 + m_31 * t_31;
                    var s = new SliceGlLine(p_12, p_31);
                    lines.Add(s);
                }
            }
            else if (p2.y > y)                                  // p2 above. p1 is not. p3 unknown.
            {
                if (p3.y > y)                                   // p2 p3 above. p1 below.
                {
                    var p_12 = p1 + m_12 * t_12;
                    var p_31 = p3 + m_31 * t_31;
                    var s = new SliceGlLine(p_12, p_31);
                    lines.Add(s);
                }
                else                                            // p2 above. p1 p3 below.
                {
                    var p_12 = p1 + m_12 * t_12;
                    var p_23 = p2 + m_23 * t_23;
                    var s = new SliceGlLine(p_12, p_23);
                    lines.Add(s);
                }
            }
            else if (p3.y > y)                                  // p3 above. p1 p2 are not.
            {
                var p_23 = p2 + m_23 * t_23;
                var p_31 = p3 + m_31 * t_31;
                var s = new SliceGlLine(p_23, p_31);
                lines.Add(s);
            }

            else continue;
        }
	}
}
