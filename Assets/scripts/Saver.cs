using UnityEngine;
using System.Collections;
using System.Windows.Forms;
using System.IO;

public class Saver
{
    public Saver()
    {
        var saveD = new SaveFileDialog();
        saveD.InitialDirectory = UnityEngine.Application.dataPath + "/Models";
        saveD.Filter = "STL Files (*.stl)|*.stl";
        saveD.RestoreDirectory = false;
        saveD.ShowDialog();
        if (saveD.FileName != "")
        {
            StreamWriter w = File.CreateText(saveD.FileName);
            w.WriteLine(saveD.FileName);
            foreach (var t in camScript.tempTriangleListBone)
            {
                var s = camScript.stlScale;
                var p1 = t.p1 / s;
                var p2 = t.p2 / s;
                var p3 = t.p3 / s;
                w.WriteLine("facet normal " + t.norm.x + " " + t.norm.y + " " + t.norm.z);
                w.WriteLine("outer loop");
                w.WriteLine("vertex " + p1.x + " " + p1.y + " " + p1.z);
                w.WriteLine("vertex " + p2.x + " " + p2.y + " " + p2.z);
                w.WriteLine("vertex " + p3.x + " " + p3.y + " " + p3.z);
                w.WriteLine("endloop");
                w.WriteLine("endfacet");
            }
            w.WriteLine("endsolid");
            w.Close();
        }
    }
}
