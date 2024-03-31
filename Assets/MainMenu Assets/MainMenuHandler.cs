using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{

    [SerializeField] public string firstLevelScene;
    [SerializeField] public string creditsLevelScene;

    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip sound;

    private int choose;

    // Start is called before the first frame update
    void Start()
    {
        choose = 0;
    }

    public void gameStart() {
        choose = 1;
        StartCoroutine(loadScene());
    }

    public void gameSettings()
    {
        Debug.Log("Implement later");
    }

    public void gameCredits()
    {
        choose = 2;
        StartCoroutine(loadScene());
    }

    public void gameQuit()
    {
        Application.Quit();
    }

    IEnumerator loadScene()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(sound);
        yield return new WaitForSeconds(3.5f);

        if (choose == 1) {
            Debug.Log("Try Load Level");
            SceneManager.LoadScene(firstLevelScene);
        }


        if (choose == 2) {
            Debug.Log("Try Load Credits");
            SceneManager.LoadScene(creditsLevelScene);
            audioSource.Play(); //TEMPORARY DUE TO CREDITS NOT BEING IMPLEMENTED
        }
    }

    public void gameStartAlt(string a)
    {
        StartCoroutine(loadSceneAlt(a));
    }

     IEnumerator loadSceneAlt(string a)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(sound);
        yield return new WaitForSeconds(3.5f);

     
            Debug.Log("Try Load Level");
            SceneManager.LoadScene(a);
        



    }

}
