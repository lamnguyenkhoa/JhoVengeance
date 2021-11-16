using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private AudioSource[] audioSources;
    private AudioClip footstep;
    private AudioClip swordSwing;
    private AudioClip swordHitBody;
    private AudioClip parry;
    private AudioClip hit;
    private AudioClip throwing;
    private AudioClip swordBlock;
    private AudioClip equip;

    public enum ImpactSoundEnum { swordSwing, swordHitBody, parry, hit, throwing, swordBlock, equip };

    // Start is called before the first frame update
    private void Start()
    {
        /* There should be 3 audioSources here.
         * audioSource[0] is for footstep sound
         * audioSource[1] is for impact sound (sword slash, projectile hit,..)
         * audioSource[2] is ...
         */
        audioSources = transform.Find("SoundStuff").GetComponents<AudioSource>();
        footstep = Resources.Load<AudioClip>("Audio/footstep");
        swordSwing = Resources.Load<AudioClip>("Audio/sword_swing");
        swordHitBody = Resources.Load<AudioClip>("Audio/sword_hit_body");
        parry = Resources.Load<AudioClip>("Audio/parry");
        hit = Resources.Load<AudioClip>("Audio/hit");
        throwing = Resources.Load<AudioClip>("Audio/throw");
        swordBlock = Resources.Load<AudioClip>("Audio/sword_block");
        equip = Resources.Load<AudioClip>("Audio/equip");
        audioSources[0].clip = footstep;
    }

    public void PlayPlayerFootstepSound()
    {
        audioSources[0].pitch = Random.Range(0.8f, 1.2f);
        audioSources[0].volume = Random.Range(0.3f, 0.5f);
        audioSources[0].Play();
    }

    public void PlayImpactSound(ImpactSoundEnum soundName)
    {
        audioSources[1].pitch = Random.Range(0.7f, 1.3f);
        audioSources[1].volume = Random.Range(0.4f, 0.6f);
        switch (soundName)
        {
            case ImpactSoundEnum.swordSwing:
                audioSources[1].PlayOneShot(swordSwing);
                break;

            case ImpactSoundEnum.swordHitBody:
                audioSources[1].PlayOneShot(swordHitBody);
                break;

            case ImpactSoundEnum.parry:
                audioSources[1].PlayOneShot(parry);
                break;

            case ImpactSoundEnum.hit:
                audioSources[1].PlayOneShot(hit);
                break;

            case ImpactSoundEnum.swordBlock:
                audioSources[1].PlayOneShot(swordBlock);
                break;

            case ImpactSoundEnum.throwing:
                audioSources[1].PlayOneShot(throwing);
                break;

            case ImpactSoundEnum.equip:
                audioSources[1].PlayOneShot(equip);
                break;

            default:
                break;
        }
    }
}