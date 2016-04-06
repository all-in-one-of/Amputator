using UnityEngine;
using System.Collections;

public class StlInterpreter
{
    private Vector3 Normal;
    public Vector3 Min { get; set; }
    public Vector3 Max { get; set; }
    bool isBone;

    public void ClearAll()
    {
        camScript.currentVertices.Clear();
        if (isBone)
        {
            foreach (var m in camScript.BoneMeshes)
            {
                MonoBehaviour.Destroy(m.gameObject);
            }
            camScript.BoneMeshes.Clear();
        }
        else
        {
            foreach (var m in camScript.SocketMeshes)
            {
                MonoBehaviour.Destroy(m.gameObject);
            }
            camScript.SocketMeshes.Clear();
        }


        Min = Vector3.one * 1000;
        Max = Vector3.one * -1000;
    }
    public Vector3 centroid
    {
        get
        {            
            var c = (Min + Max) / 2.0f;
            return c;
        }
    }

    public StlInterpreter(bool _isBone)
    {
        Min = Vector3.one * 1000;
        Max = Vector3.one * -1000;
        isBone = _isBone;
    }

    public void normal (string _line)
    {
        float x;
        float y;
        float z;
        var split = _line.Split('l');
        split[1].TrimStart(' ');
        var coords = split[1].Split(' ');
        var xString = coords[0];
        var yString = coords[1];
        var zString = coords[2];
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
        Normal = new Vector3(x, y, z);
    }

    public void outerloop()
    {
    }

	public void endloop (string _line)
	{
		try
		{
            var p = camScript.currentVertices;
            //mm.AddTriangle(p[0], p[1], p[2], Normal, false);
            var t = new Triangle(p[0], p[1], p[2], Normal, false);
            if (isBone)
                camScript.triangleListBone.Add(t);
            else
                camScript.triangleListSocket.Add(t);
            Camera.main.GetComponent<camScript>().SetMaxMin(p[0], false, isBone);
            Camera.main.GetComponent<camScript>().SetMaxMin(p[1], false, isBone);
            Camera.main.GetComponent<camScript>().SetMaxMin(p[2], false, isBone);
            camScript.currentVertices.Clear ();
		}
		catch{}
	}

	public void vertex (string _line)
	{
		{
			float x;
			float y;
			float z;
			var coordSep = _line.Split('x');
			var coords = coordSep[1].TrimStart(' ').Split(' ');
			
			var xString = coords[0];
			var yString = coords[1];
			var zString = coords[2];
			var xStrSplit = xString.Split ('e');
			if (float.TryParse(xStrSplit[0], out x))
			{
				float xE;
				if (xStrSplit.Length > 1 && float.TryParse(xStrSplit[1], out xE))
					x *= (Mathf.Pow (10f, xE));
			}
			
			var yStrSplit = yString.Split ('e');
			if (float.TryParse(yStrSplit[0], out y))
			{
				float yE;
				if (yStrSplit.Length > 1 && float.TryParse(yStrSplit[1], out yE))
					y *= (Mathf.Pow (10f, yE));
			}
			
			var zStrSplit = zString.Split ('e');
			if (float.TryParse(zStrSplit[0], out z))
			{
				float zE;
				if (zStrSplit.Length > 1 && float.TryParse(zStrSplit[1], out zE))
					z *= (Mathf.Pow (10f, zE));
			}
			var newVertex = new Vector3 (x,y,z) * camScript.stlScale;
            Camera.main.GetComponent<camScript>().SetMaxMin(newVertex, false, isBone);

            camScript.currentVertices.Add (newVertex);
		}
	}
}
