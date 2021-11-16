using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string[] possibleNextScene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() || other.GetComponent<PlayerControllerZen>())
        {
            int nextSceneId = Random.Range(0, possibleNextScene.Length);
            if (SceneManager.GetActiveScene().name == "ZenGarden")
            {
                PlayerStatHUD.psh.ResetToGoDungeon();
            }
            if (SceneManager.GetActiveScene().name == "RoomTutorial")
            {
                PlayerStatHUD.psh.ResetToGoZen();
            }
            SceneManager.LoadScene(possibleNextScene[nextSceneId]);
        }
    }
}