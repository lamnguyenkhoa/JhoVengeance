using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraView : MonoBehaviour
{
    public float height;
    public float zOffset;
    public bool hasRotate;
    public Vector3 rotation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
            cam.height = height;
            cam.zOffset = zOffset;
            if (hasRotate)
            {
                StartCoroutine(cam.RotateMe(rotation, 0.8f));
            }
        }
    }
}