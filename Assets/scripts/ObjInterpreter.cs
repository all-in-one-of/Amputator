using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

public class ObjInterpreter
{
    List<Vector3> verts = new List<Vector3>();
    List<Vector3> norms = new List<Vector3>();
    List<Vector3> uvs = new List<Vector3>();
    List<Vector3> indices = new List<Vector3>();
    float objScale = 1f;
    int _tCount = 0;
    int _vCount = 0;

    public ObjInterpreter(string _filename, bool _isBone)
    {
        var reader = new StreamReader(_filename);
        while (!reader.EndOfStream)
        {
            var s = reader.ReadToEnd();
            s = s.Replace("  ", " ");
            s = s.Replace("  ", " ");
            string[] lines = s.Split("\n"[0]);
            foreach (string item in lines)
            {
                ReadLine(item, _isBone);
            }
        }

        foreach (var i in indices)
        {
            var _p1 = (int)i.x - 1;
            var _p2 = (int)i.y - 1;
            var _p3 = (int)i.z - 1;
            if (verts.Count <= _p1 || verts.Count <= _p2 || verts.Count <= _p3) continue;
            var p1 = verts[_p1];
            var p2 = verts[_p2];
            var p3 = verts[_p3];
            var n = Normal(p1, p2, p3);
            if (norms.Count > _p1)
            {
                n = norms[_p1];
            }
            var t = new Triangle(p1, p2, p3, n, false);
            if (_isBone)
                camScript.triangleListBone.Add(t);
            else
                camScript.triangleListSocket.Add(t);
        }
        var c = (camScript.MinSocket + camScript.MaxSocket) / 2.0f;
        if (_isBone)
            c = (camScript.MinBone + camScript.MaxBone) / 2.0f;
        //foreach (var tri in camScript.triangleListBone)
        //{            
        //    tri.p1 -= c;
        //    tri.p2 -= c;
        //    tri.p3 -= c;
        //}
        Camera.main.GetComponent<camScript>().Generate(_isBone);
    }

    public Vector3 Normal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var dir = Vector3.Cross(p2 - p1, p3 - p1);
        var norm = Vector3.Normalize(dir);
        return norm;
    }

    public void ReadLine(string s, bool _isBone)
    {
        //remove any trailing white-space chararcters to ensure that there will be no empty splits
        char[] charsToTrim = { ' ', '\n', '\t', '\r' };
        s = s.TrimEnd(charsToTrim);
        //split the incoming string in words
        string[] words = s.Split(" "[0]);
        //trim each word to avoid white-space chararcters
        foreach (string item in words)
            item.Trim();
        //assemble all vertices, normals and uv-coordinates
        if (words[0] == "v")
        {
            verts.Add(getVector(words[1], words[2], words[3], _isBone));
            uvs.Add(getVector(words[1], words[2], words[3], _isBone));
            _vCount++;
        }

        if (words[0] == "vn")
        {
            norms.Add(getVector(words[1], words[2], words[3], _isBone));
        }
        if (words[0] == "vt")
        {
        }
        //assemble the faces by index, and disassemble them back to each point
        if (words[0] == "f")
        {
            _tCount++;
            var indi1 = words[1].Split("/"[0])[0];
            var indi2 = words[2].Split("/"[0])[0];
            var indi3 = words[3].Split("/"[0])[0];
            float i1;
            float i2;
            float i3;
            if (float.TryParse(indi1, out i1) && float.TryParse(indi2, out i2) && float.TryParse(indi3, out i3))
            {
                var i = new Vector3(i1, i2, i3);
                indices.Add(i);
            }
        }
    }

    public Vector3 getVector(string xString, string yString, string zString, bool _isBone)
    {
        {
            float x;
            float y;
            float z;
            var xStrSplit = xString.Split('e');
            if (float.TryParse(xStrSplit[0], out x))
            {
                float xE;
                if (xStrSplit.Length > 1 && float.TryParse(xStrSplit[1], out xE))
                    x *= (Mathf.Pow(10f, xE));
            }

            var yStrSplit = yString.Split('e');
            if (float.TryParse(yStrSplit[0], out y))
            {
                float yE;
                if (yStrSplit.Length > 1 && float.TryParse(yStrSplit[1], out yE))
                    y *= (Mathf.Pow(10f, yE));
            }

            var zStrSplit = zString.Split('e');
            if (float.TryParse(zStrSplit[0], out z))
            {
                float zE;
                if (zStrSplit.Length > 1 && float.TryParse(zStrSplit[1], out zE))
                    z *= (Mathf.Pow(10f, zE));
            }
            var newVertex = new Vector3(x, y, z) * objScale;
            Camera.main.GetComponent<camScript>().SetMaxMin(newVertex, false, _isBone);

            return newVertex;
        }
    }
}
