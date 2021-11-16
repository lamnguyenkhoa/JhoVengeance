using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private AudioSource audioSource;
    public static MusicManager musicManager;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (!musicManager)
        {
            musicManager = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // If there is another instance, check that if it has different bgm.
            // If it has, then play that instead.
            if (musicManager.audioSource.clip != this.audioSource.clip)
            {
                musicManager.audioSource.clip = this.audioSource.clip;
                musicManager.audioSource.Play();
            }
            Destroy(this.gameObject);
        }
    }

    public void PlayMusic()
    {
        if (audioSource.isPlaying) return;
        audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void PlayVictoryTheme()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        AudioClip victoryTheme = Resources.Load<AudioClip>("Audio/victory_theme");
        audioSource.clip = victoryTheme;
        audioSource.volume = 1f;
        audioSource.Play();
    }
}