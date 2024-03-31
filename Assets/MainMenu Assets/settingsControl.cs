using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class settingsControl : MonoBehaviour
{

    public static float sens;
    // Start is called before the first frame update
    void Start()
    {   
        if(GameManager.sensitivity != 0){
         sens = GameManager.sensitivity;   
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void restartLevel() {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }


    public void volumeGo() {
        float volume = GameObject.Find("Volume Slider").GetComponent<Slider>().value;

        GameManager.volume = volume;
    }

    public void sensGo()
    {
        sens = GameObject.Find("Sens Slider").GetComponent<Slider>().value;
        GameManager.sensitivity = sens;
    }
}
