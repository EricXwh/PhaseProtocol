using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacetoTransitionHunter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Application.LoadLevel("Zone1 (Final)");
        }
    }
}
