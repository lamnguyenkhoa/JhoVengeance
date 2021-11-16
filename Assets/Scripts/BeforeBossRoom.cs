using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeforeBossRoom : MonoBehaviour
{
    public float shakeDuration;
    public float shakeMagnitude;
    private bool runOnce = false;
    private GameObject boss;
    private GameObject bossLight;
    public GameObject bossGate;
    public GameObject[] blockViewObjects;

    private void Awake()
    {
        boss = GameObject.Find("BossLongNeck").gameObject;
        bossLight = GameObject.Find("BossLight").gameObject;
        boss.SetActive(false);
        bossLight.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.GetComponent<PlayerController>() && !runOnce)
        {
            boss.SetActive(true);
            bossLight.SetActive(true);
            transform.GetComponent<AudioSource>().Play();
            runOnce = true;
            CameraFollow cam = Camera.main.transform.GetComponent<CameraFollow>();
            bossGate.GetComponent<BossGate>().CloseGate();
            StartCoroutine(ChangeCameraHeight(cam, shakeDuration));
            StartCoroutine(cam.Shake(shakeDuration, shakeMagnitude));

            // Remove object/wall that might block player view
            foreach (GameObject blockObject in blockViewObjects)
            {
                blockObject.SetActive(false);
            }
        }
    }

    // One time use for this specific script
    private IEnumerator ChangeCameraHeight(CameraFollow cam, float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime + 1f);
        cam.height = 12;
        cam.zOffset = -14f;
        Destroy(this);
    }
}