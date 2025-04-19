using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] AudioSource buttonClicked;
    bool tutorial = false;
    bool exit = false;
    public void moveToTutorialRoom()
    {
        Debug.Log("Tutorial Started!");
        buttonClicked.Play();
        tutorial = true;
        StartCoroutine(WaitForSoundToFinish());
    }
    
    public void exitGame()
    {
        Debug.Log("Game Closed!");
        buttonClicked.Play();
        exit = true;
        StartCoroutine(WaitForSoundToFinish());
    }

    public void startGame()
    {
        buttonClicked.Play();
        StartCoroutine(WaitForSoundToFinish());
    }
    IEnumerator WaitForSoundToFinish()
    {
        yield return new WaitUntil(() => !buttonClicked.isPlaying);
        if(tutorial)
        {
            SceneManager.LoadSceneAsync("Tutorial Scene");
        }
        else if(exit)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadSceneAsync("JakeScene"); //changed from sample scene
        }
    }
}
