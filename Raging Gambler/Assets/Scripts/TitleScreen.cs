using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void moveToTutorialRoom()
    {
        Debug.Log("Tutorial Started!");
        SceneManager.LoadScene("Tutorial Scene");
    }
    
    public void exitGame()
    {
        Debug.Log("Game Closed!");
        Application.Quit();
    }

    public void startGame()
    {
        SceneManager.LoadSceneAsync("JakeScene"); //changed from sample scene
    }
}
