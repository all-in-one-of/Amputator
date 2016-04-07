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
    public UnityEngine.UI.Button sliceButton;
    public MakeMesh MM;
    public Transform boneHolder;
    public Transform socketHolder;
    public Material BoneMat;
    public Material SocketMat;
    public MeshSimplifier simplifier;
    public float Precision;
    public static List<MakeMesh> BoneMeshes = new List<MakeMesh>();
    public static List<MakeMesh> SocketMeshes = new List<MakeMesh>();
    public static List<Triangle> triangleListBone = new List<Triangle>();
    public static List<Triangle> tempTriangleListBone = new List<Triangle>();
    public static List<Triangle> triangleListSocket = new List<Triangle>();
    public static List<Triangle> tempTriangleListSocket = new List<Triangle>();
    public static List<Vector3> currentVertices = new List<Vector3>();

    public GameObject mainMenu;
    private StlInterpreter stlInterpreter;
    public static Vector3 MinBone = Vector3.one * 10000;
    public static Vector3 MaxBone = Vector3.one * -10000;
    public static Vector3 MinSocket = Vector3.one * 10000;
    public static Vector3 MaxSocket = Vector3.one * -10000;
    public static Vector3 tmpMinBone = Vector3.one * 10000;
    public static Vector3 tmpMaxBone = Vector3.one * -10000;
    public static Vector3 tmpMinSocket = Vector3.one * 10000;
    public static Vector3 tmpMaxSocket = Vector3.one * -10000;
    public static Parse_StlBinary parseStlBinary;
    public static Color stlColor = Color.white;
    public static float stlScale = 100;
    public GameObject slicePlane;
    public static bool slice = false;

    void Start()
    {
        var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -1f));
        var rot = new Quaternion(0, 0, 0, 0);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            UnityEngine.Application.Quit();
        var pos = slicePlane.transform.position;
        if (slice)
            pos.y = move_plane.value;
        else if (pos.y != -100000) pos.y = -100000;
        slicePlane.transform.position = pos;
    }

	void OnGUI()
    {
    }

    public void ToggleSlicer()
    {
        slice =  slicerToggle.isOn;
        if (slice)
        {
            move_plane.interactable = true;
            sliceButton.interactable = true;
        }
        else
        {
            move_plane.interactable = false;
            sliceButton.interactable = false;
        }
    }
    public void loadFile(bool _isBone)
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
                if (_isBone)
                {
                    triangleListBone.Clear();
                    tempTriangleListBone.Clear();
                    stlInterpreter = new StlInterpreter(_isBone);
                }
                else
                {
                    triangleListSocket.Clear();
                    tempTriangleListSocket.Clear();
                    stlInterpreter = new StlInterpreter(_isBone);
                }
                var fileName = openFileDialog.FileName;
                if (fileName.ToLower().EndsWith(".obj"))
                {
                    var objI = new ObjInterpreter(fileName, _isBone);
                    return;
                }
                stlInterpreter.ClearAll();
                linesOfStl.Clear();
                if (CheckForStlBinary(fileName))
                {
                    parseStlBinary = new Parse_StlBinary(fileName, null, _isBone);
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
                    if (_isBone)
                    {
                        foreach (var tri in triangleListBone)
                        {
                            var c = (camScript.MinBone + camScript.MaxBone) / 2.0f;
                            tri.p1 -= c;
                            tri.p2 -= c;
                            tri.p3 -= c;
                        }
                    }
                    else
                    {
                        foreach (var tri in triangleListSocket)
                        {
                            var c = (camScript.MinSocket + camScript.MaxSocket) / 2.0f;
                            tri.p1 -= c;
                            tri.p2 -= c;
                            tri.p3 -= c;
                        }
                    }
                    Generate(_isBone);
                }
            }
            catch { }
            }
    }

    public void Generate(bool _isBone)
    {
        var simplifier = new MeshSimplifier(_isBone);
        var splitter = new Splitter(_isBone);
    }
    public void Redraw(bool _isBone)
    {
        var fullTempTriList = new List<Triangle>();
        if (_isBone)
        {
            foreach (var m in BoneMeshes)
            {
                Destroy(m.gameObject);
            }
            BoneMeshes.Clear();
            fullTempTriList = camScript.tempTriangleListBone;
        }
        else
        {
            foreach (var m in SocketMeshes)
            {
                Destroy(m.gameObject);
            }
            SocketMeshes.Clear();
            fullTempTriList = camScript.tempTriangleListSocket;
        }
        var maxPts = 20000;
        var numVerts = tempTriangleListSocket.Count * 3;
        if (_isBone)
            numVerts = tempTriangleListBone.Count * 3;
        var numMeshes = Mathf.CeilToInt((float)numVerts / maxPts);
        var tmp = new List<List<Triangle>>();
        for (int i = 0; i < numMeshes; i++)
        {
            var tl = new List<Triangle>();
            var vi = i * maxPts;
            for (int j = vi; j < vi + maxPts; j++)
            {
                if (fullTempTriList.Count > j)
                    tl.Add(fullTempTriList[j]);
            }
            tmp.Add(tl);
        }
        foreach (var l in tmp)
        {
            var mm = Instantiate(MM) as MakeMesh;
            mm.ClearAll();
            if (_isBone)
                mm.material = BoneMat;
            else
                mm.material = SocketMat;
            mm.Begin();
            foreach (var tri in l)
            {
                SetMaxMin(tri.p1, true, _isBone);
                SetMaxMin(tri.p2, true, _isBone);
                SetMaxMin(tri.p3, true, _isBone);
                mm.AddTriangle(tri.p1, tri.p2, tri.p3, tri.norm, tri._binary);
            }
            mm.MergeMesh();
            if (_isBone)
            {
                mm.gameObject.transform.SetParent(boneHolder);
                BoneMeshes.Add(mm);
            }
            else
            {
                mm.gameObject.transform.SetParent(socketHolder);
                SocketMeshes.Add(mm);
            }
        }
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

    public void SetMaxMin (Vector3 vert, bool temp, bool _isBone)
    {
        var max = MaxSocket;
        var min = MinSocket;
        if (_isBone)
        {
            max = MaxBone;
            min = MinBone;
        }
        if (vert.x > max.x) max.x = vert.x;
        if (vert.x < min.x) min.x = vert.x;
        if (vert.y > max.y) max.y = vert.y;
        if (vert.y < min.y) min.y = vert.y;
        if (vert.z > max.z) max.z = vert.z;
        if (vert.z < min.z) min.z = vert.z;
        if (_isBone)
        {
            MaxBone = max;
            MinBone = min;
        }
        else
        {
            MaxSocket = max;
            MinSocket = min;
        }
    }

    public void Save()
    {
        var saver = new Saver();
    }
}
