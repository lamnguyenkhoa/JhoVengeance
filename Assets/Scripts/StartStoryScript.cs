using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartStoryScript : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            GoBackToTitle();
        }
    }

    public void GoBackToTitle()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("RoomTutorial");
    }
}