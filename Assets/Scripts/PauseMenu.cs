using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject controlsMenuUI;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                if (PlayerStatHUD.psh)
                {
                    if (!PlayerStatHUD.psh.disablePause)
                        Pause();
                }
                else
                {
                    Pause();
                }
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        controlsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Pause()
    {
        if (!PlayerController.isInInventory)
        {
            pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
        }
    }

    public void PlayButton()
    {
        // Force player to go through story, then tutorial.
        SceneManager.LoadScene("StartStory");
    }

    //public void TutorialButton()
    //{
    //    SceneManager.LoadScene("RoomTutorial");
    //}

    public void QuitGameButton()
    {
        Debug.Log("game quit");
        Application.Quit();
    }

    public void SepukuButton()
    {
        PlayerController player = GameObject.Find("PlayerDino").GetComponent<PlayerController>();
        PlayerStatHUD.psh.SetHealth(0);
        player.playerAudio.PlayImpactSound(PlayerAudio.ImpactSoundEnum.hit);
        player.Death();
        Resume();
    }

    //public void StoryButton()
    //{
    //    SceneManager.LoadScene("StartStory");
    //}
}