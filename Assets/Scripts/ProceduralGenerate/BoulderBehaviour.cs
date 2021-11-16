using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderBehaviour : MonoBehaviour
{
    private ProcGenBoulder boulder;
    private SphereCollider coll;
    private AudioSource audioSource;

    public Vector3 velocity;
    public bool isGrounded;
    public int damage = 10;
    public bool canDealDamage = true;
    public LayerMask groundLayer;

    // Start is called before the first frame update
    private void Start()
    {
        boulder = GetComponent<ProcGenBoulder>();
        coll = GetComponent<SphereCollider>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isGrounded)
        {
            isGrounded = Physics.CheckSphere(transform.position + new Vector3(0, 3f, 0), 1f, groundLayer, QueryTriggerInteraction.Ignore);
            if (isGrounded)
            {
                // CRASH!
                coll.radius *= 1.5f;
                transform.localScale += new Vector3(0.5f, 0.5f, 0.5f);
                velocity.y = 0f;
                boulder.nDivision = 5;
                boulder.randomMagnitude = 3;
                boulder.ReGenerate();
                audioSource.Play();
                StartCoroutine(Camera.main.transform.GetComponent<CameraFollow>().Shake(0.3f, 0.2f));
                StartCoroutine(EarthQuakeGoUpAndDown());
            }
            velocity.y += Physics.gravity.y * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
        }
    }

    private IEnumerator EarthQuakeGoUpAndDown()
    {
        float startTime = Time.time;
        while (Time.time < startTime + 0.2f)
        {
            transform.position += Vector3.up * 6 * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        canDealDamage = false;
        while (Time.time < startTime + 3f)
        {
            transform.position += Vector3.down * 4 * Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (canDealDamage)
        {
            PlayerController player = collision.transform.GetComponent<PlayerController>();
            if (player)
            {
                player.Damaged(damage, transform.position);
                canDealDamage = false;
            }
        }
    }
}