using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void moveToTutorialRoom()
    {
        Debug.Log("Tutorial Started!");
        //MAKE TUTORIAL SCENE
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
