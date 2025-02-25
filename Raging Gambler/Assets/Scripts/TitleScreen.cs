using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void moveToTutorialRoom()
    {
        SceneManager.LoadSceneAsync("SampleScene");
    }
    
    public void exitGame()
    {
        Debug.Log("Game Closed!");
        Application.Quit();
    }

    public void startGame()
    {
        Debug.Log("Game started!");
    }
}
