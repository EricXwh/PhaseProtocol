using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PauseGame : MonoBehaviour
{

    private bool paused = false;
    [SerializeField] public GameObject buttons;
    [SerializeField] public GameObject UI;
    [SerializeField] public CinemachineBrain CameraMachine;

    private bool canPause = false;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        if (CameraMachine == null) {
            Debug.Log("SET CAMERA FOR PAUSE LISTENER!!");
        }

       buttons.SetActive(false);
       canPause = true;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (canPause == true) {
            Pause();
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && paused == false)
        {
            canPause = false;
            Pause();
        }

        if (paused == true) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }




    }

    public void Pause() {
        if (paused == false) // Pause turn on
        {
            buttons.SetActive(true);
            CameraMachine.enabled = false;
            //UI.SetActive(false);
            if (SceneManager.GetActiveScene().name.Equals("Zone0"))
            {
                AudioListener.pause = true;
                UI.GetComponent<CanvasGroup>().alpha = 0;
            }
            Time.timeScale = 0;
            paused = true;
        }
        else { // Pause turn off
            buttons.SetActive(false);
            CameraMachine.enabled = true;
            //UI.SetActive(true);
            if (SceneManager.GetActiveScene().name.Equals("Zone0"))
            {
                UI.GetComponent<CanvasGroup>().alpha = 1;
                AudioListener.pause = false;
            }
            Time.timeScale = 1;
            paused = false;
            canPause = true;

        }
        
    }

    public void MainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }
}
