using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class ProjectileController : MonoBehaviour
{
    /* General attributes */
    public GameObject owner;
    public int damage = 0; // damage will be decided by player/enemy stat
    public float lifeTime;
    public Vector3 spinValue = Vector3.zero;
    public bool friendlyFire;
    public bool collideWithOtherProj;
    public Vector3 velocity;
    public Vector3 throwDirection;
    public float throwPower = 30f;
    public int parryMultiplier = 2;

    /* Special attributes */
    public bool bounce;
    public int maxCollisions = 1;
    public bool hasArcMotion;
    public Vector3 arcDestination;
    public GameObject explosionPrefab;
    public int explosionRange;

    /* Internal variables */
    private int collisionCounter;
    private Rigidbody rb;
    private PhysicMaterial physicsMat;

    /* Other */
    public AudioClip hitSound;
    public AudioClip explodeSound;
    public AudioClip throwSound;
    public LayerMask Enemies;
    public AudioMixer mixer;

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
        physicsMat = transform.GetComponent<PhysicMaterial>();
    }

    private void Start()
    {
        PlayProjectileSound(throwSound);
        throwDirection += owner.transform.forward;
        rb = transform.GetComponent<Rigidbody>();
        if (hasArcMotion)
        {
            // The player mouse location will be the destination. Calculate force and angle
            // to apply to projectiles so it arrived accurate there
        }
        else
        {
            // Straight line throw
            rb.AddForce(throwDirection * throwPower, ForceMode.Impulse);
        }
    }

    private void Update()
    {
        this.transform.Translate(velocity * Time.deltaTime);
        transform.Rotate(spinValue);
        if (collisionCounter > maxCollisions) Explode();

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            if (explosionPrefab)
            {
                Explode();
            }
            else
            {
                Destroy(this.gameObject);
            }
        };
    }

    private void OnCollisionEnter(Collision col)
    {
        // Prevent self-collision
        if (col.gameObject == owner)
        {
            return;
        }

        // No collision with trigger
        if (col.gameObject.GetComponent<Collider>().isTrigger)
        {
            return;
        }

        // Make hit noise
        if (hitSound)
        {
            PlayProjectileSound(hitSound);
        }

        PlayerController player = col.transform.GetComponent<PlayerController>();
        IEnemy enemy = col.transform.GetComponent<IEnemy>();

        // If it hit player, deal damage to player
        if (player)
        {
            if (damage > 0)
            {
                if (explosionPrefab)
                {
                    Explode();
                    return;
                }
                else
                {
                    if (damage > 0) player.Damaged(damage, owner.transform.position);
                }
            }
        }
        // Hit enemies
        else if (enemy)
        {
            // from another enemies, depend on whether we want friendly fire or
            // not it will deal damage
            if (owner.transform.GetComponent<IEnemy>() && friendlyFire)
            {
                if (explosionPrefab)
                {
                    Explode();
                    return;
                }
                else
                {
                    if (damage > 0) enemy.Damaged(damage, 0, owner.transform.position);
                }
            }
            if (owner.transform.GetComponent<PlayerController>())
            {
                if (explosionPrefab)
                {
                    Explode();
                    return;
                }
                else
                {
                    if (damage > 0) enemy.Damaged(damage, 0, owner.transform.position);
                }
            }
        }
        // Hit others projectiles
        else if (col.transform.GetComponent<ProjectileController>())
        {
            if (!collideWithOtherProj)
            {
                // Not destroy
                return;
            }
            if (explosionPrefab) Explode();
        }

        // If reached here, then it hit the environment

        if (!bounce)
        {
            if (explosionPrefab)
            {
                Explode();
            }
            Destroy(this.gameObject);
        }
        else
        {
            collisionCounter++;
        }
    }

    private void Explode()
    {
        if (explosionPrefab)
        {
            GameObject tmp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, Enemies);
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].GetComponent<IEnemy>().Damaged(damage, 0, owner.transform.position);
            }
            if (explodeSound)
            {
                PlayProjectileSound(explodeSound);
            }
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// This function allow the played sound to be at big volume while still
    /// maintain the 3D aspect of the sound. Use PlayClipAtPoint normally will give
    /// a very small sound.
    /// </summary>
    /// <param name="soundClip"></param>
    private void PlayProjectileSound(AudioClip soundClip)
    {
        float tmp;
        mixer.GetFloat("OverallVolume", out tmp);
        tmp = tmp / 20;
        tmp = Mathf.Pow(10, tmp);

        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 soundDirection = transform.position - cameraPos;
        AudioSource.PlayClipAtPoint(soundClip, cameraPos + soundDirection.normalized, tmp);
    }
}