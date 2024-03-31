using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Simple scene switching for players of MVP2
public class SceneSwap : MonoBehaviour
{
    //[SerializeField] BasePlayer player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Press 0 for Main Menu
            SceneManager.LoadScene("Main Menu");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Press 2 for Level 0
            SceneManager.LoadScene("Zone0");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //Press 3 for Level 1
            SceneManager.LoadScene("Zone1 (Final)");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //Press 4 for Zone 2 (not yet completeable)
            SceneManager.LoadScene("Zone2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            //Press 5 for Zone3
            SceneManager.LoadScene("Zone3");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            //Press 6 for EndCard
            SceneManager.LoadScene("End Card");
        }
    }
}
