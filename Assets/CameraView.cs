using UnityEngine;
using System.Collections;

public class CameraView : MonoBehaviour
{
    float scaleRot = 0.1f;
    Vector3 lastMp;


	void Start ()
    {
        lastMp = Input.mousePosition;
    }
	
	void Update ()
    {
        var c = (camScript.tmpMinBone + camScript.tmpMaxBone) / 2.0f;
        if (GameObject.Find("Camera Rig").transform.position != c)
        {
            GameObject.Find("Camera Rig").transform.position = c;
        }

        var mp = Input.mousePosition;
        if (mp.x < 250)
        {
            lastMp = mp;
            return;
        }
        var xFactor = (mp.x - lastMp.x) * scaleRot;
        var yFactor = -(mp.y - lastMp.y) * scaleRot;

        if (Input.GetMouseButton(0))
        {

            GameObject.Find("Camera Rig").transform.RotateAround(c, Camera.main.transform.up, xFactor);
            GameObject.Find("Camera Rig").transform.RotateAround(c, Camera.main.transform.right, yFactor);
        }
        var zoom = Input.mouseScrollDelta.y * 0.5f;
        var pos = Camera.main.transform.localPosition;
        pos.z += zoom;
        Camera.main.transform.localPosition = pos;
        lastMp = mp;
    }
}
