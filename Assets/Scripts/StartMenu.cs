using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject controlsMenuUI;

    // Update is called once per frame
    


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
       
    }

    //public void StoryButton()
    //{
    //    SceneManager.LoadScene("StartStory");
    //}
}