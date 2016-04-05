using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Windows.Forms;


public class camScript : MonoBehaviour
{
    public static List<string> linesOfStl = new List<string>();
    
    public Slider move_plane;
    public Dropdown limb_select;
    public Toggle slicerToggle;
    public static MakeMesh MM;
    public static List<MakeMesh> BoneMeshes = new List<MakeMesh>();
    public static List<MakeMesh> SocketMeshes = new List<MakeMesh>();

    public GameObject mainMenu;
    private StlInterpreter stlInterpreter;
    public static List<Vector3> currentVertices = new List<Vector3>();
    public static Vector3 Min = new Vector3(1000, 1000, 1000);
    public static Vector3 Max = new Vector3(-1000, -1000, -1000);
    public static Parse_StlBinary parseStlBinary;
    public static List<Triangle> triangleList = new List<Triangle>();
    public static List<Triangle> tempTriangleList = new List<Triangle>();
    public static List<Triangle> triangleListToDraw = new List<Triangle>();
    public Material Mat;
    public static Color stlColor = Color.white;
    public static float stlScale = 100;
    public GameObject slicePlane;
    public static Vector3 tmpMin = Vector3.one * 10000;
    public static Vector3 tmpMax = Vector3.one * -10000;
    bool slice = false;

    void Start()
    {
        stlInterpreter = new StlInterpreter();
        var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
        var rot = new Quaternion(0, 0, 0, 0);
        MM = GameObject.Find("BONE").GetComponent<MakeMesh>();
        MM.material = Mat;
        MM.Begin();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            UnityEngine.Application.Quit();
        var pos = slicePlane.transform.position;
        pos.y = move_plane.value;
        slicePlane.transform.position = pos;
    }

	void OnGUI()
    {
    }

    public void ToggleSlicer()
    {
        if (slicerToggle.isOn)
            
        print(slice.ToString());
    }
    public void loadFile()
    {
        System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
        openFileDialog.InitialDirectory = UnityEngine.Application.dataPath + "/Models";
        var sel = "STL Files (*.STL)|*.STL|Obj Files (*.OBJ, *.obj)|*.obj";
        openFileDialog.Filter = sel;
        openFileDialog.FilterIndex = 1;
        openFileDialog.RestoreDirectory = false;

        if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            try
            {
                triangleList.Clear();
                tempTriangleList.Clear();
                triangleListToDraw.Clear();
                var fileName = openFileDialog.FileName;
                if (fileName.ToLower().EndsWith(".obj"))
                {
                    var objI = new ObjInterpreter(fileName);
                    return;
                }
                stlInterpreter.ClearAll();
                linesOfStl.Clear();
                if (CheckForStlBinary(fileName))
                {
                    parseStlBinary = new Parse_StlBinary(fileName, MM, null);
                }
                else
                {
                    var reader = new StreamReader(fileName);
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadToEnd();
                        line = line.Replace("facet", "|facet");
                        line = line.Replace("outer loop", "|outer loop");
                        line = line.Replace("endloop", "|endloop");
                        line = line.Replace("vertex", "|vertex");
                        line = line.Replace("endfacet", "|endfacet");
                        var _lines = line.Split('|');
                        foreach (var _line in _lines)
                        {
                            linesOfStl.Add(_line);
                        }
                    }
                    foreach (var l in linesOfStl)
                    {
                        scanSTL(l);
                    }
                    foreach (var tri in triangleList)
                    {
                        var c = (camScript.Min + camScript.Max) / 2.0f;
                        tri.p1 -= c;
                        tri.p2 -= c;
                        tri.p3 -= c;
                    }
                    Generate();
                }
            }
            catch { }
            }
    }

    public void Generate()
    {
        var splitter = new Splitter();
    }
    public void Redraw()
    {
        MM.ClearAll();
        MM.Begin();
        foreach (var tri in tempTriangleList)
        {
            TmpSetMaxMin(tri.p1);
            TmpSetMaxMin(tri.p2);
            TmpSetMaxMin(tri.p3);
            MM.AddTriangle(tri.p1, tri.p2, tri.p3, tri.norm, tri._binary);
        }
        MM.MergeMesh();
    }
    public void scanSTL(string _line)
    {
        _line = _line.Trim();
        if (_line.Contains("outer"))
        {
            currentVertices.Clear();
            stlInterpreter.outerloop();
        }
        else if (_line.Contains("endloop"))
        {
            stlInterpreter.endloop(_line);
        }
        else if (_line.Contains("vertex"))
        {
            stlInterpreter.vertex(_line);
        }

        else if (_line.Contains("normal"))
        {
            stlInterpreter.normal(_line);
        }
    }

    public bool CheckForStlBinary(string _path)
    {
        var _isBinary = false;
        var readBytesArray = File.ReadAllBytes(_path);
        var numBytes = readBytesArray.Length;
        if (numBytes < 84)
            return false;
        var numFacets = new byte[4];
        for (int i = 80; i < 84; i++)
        {
            numFacets[i - 80] = readBytesArray[i];
        }
        var num_facets = BitConverter.ToInt32(numFacets, 0);
        var predictedNumBytes = (84 + 50 * num_facets);
        if (numBytes == predictedNumBytes)
        {
            _isBinary = true;
        }
        else
        {
            _isBinary = false;
        }
        return _isBinary;
    }

    public void SetMaxMin (Vector3 vert)
    {
        var max = Max;
        var min = Min;
        if (vert.x > Max.x) max.x = vert.x;
        if (vert.x < Min.x) min.x = vert.x;
        if (vert.y > Max.y) max.y = vert.y;
        if (vert.y < Min.y) min.y = vert.y;
        if (vert.z > Max.z) max.z = vert.z;
        if (vert.z < Min.z) min.z = vert.z;
        Max = max;
        Min = min;
    }

    public void TmpSetMaxMin(Vector3 vert)
    {
        var max = tmpMax;
        var min = tmpMin;
        if (vert.x > max.x) max.x = vert.x;
        if (vert.x < min.x) min.x = vert.x;
        if (vert.y > max.y) max.y = vert.y;
        if (vert.y < min.y) min.y = vert.y;
        if (vert.z > max.z) max.z = vert.z;
        if (vert.z < min.z) min.z = vert.z;
        tmpMax = max;
        tmpMin = min;
    }

    public Vector3 tmpCentroid ()
    {
        return (tmpMax + tmpMin) / 2;
    }

    public void Save()
    {
        var saver = new Saver();
    }
}
