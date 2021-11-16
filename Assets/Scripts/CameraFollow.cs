using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smooth = 0.3f;
    public float height;
    public float zOffset;

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.Find("PlayerDino").transform;
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y < 0)
        {
            if (height <= 12f)
            {
                height += 0.2f;
                zOffset -= 0.2f;
            }
        }

        if (Input.mouseScrollDelta.y > 0)
        {
            if (height >= 4f)
            {
                height -= 0.2f;
                zOffset += 0.2f;
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            // Default value
            height = 8f;
            zOffset = -10f;
        }

        Vector3 pos = new Vector3();
        pos.x = player.position.x;
        pos.y = player.position.y + height;
        pos.z = player.position.z + zOffset;
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smooth);
    }

    public IEnumerator RotateMe(Vector3 angles, float inTime)
    {
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(angles);
        for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
        {
            transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }
    }

    /// <summary>
    /// Use StartCoroutine. Shake the screen.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="magnitude"></param>
    /// <returns></returns>
    public IEnumerator Shake(float duration, float magnitude)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position += new Vector3(x, y, 0);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}