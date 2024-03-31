using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Just used to define Player instances */
public class BasePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Mangers -> " + GameObject.FindObjectsOfType(typeof(GameManager)).Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
