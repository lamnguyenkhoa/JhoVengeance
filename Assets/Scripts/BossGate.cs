using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGate : MonoBehaviour
{
    public float yCoord;

    public void CloseGate()
    {
        transform.position = new Vector3(transform.position.x, yCoord, transform.position.z);
    }
}