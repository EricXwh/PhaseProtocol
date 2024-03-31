using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grate : BaseObstacle
{

    [SerializeField] GameObject GrateHitbox;

    void OnTriggerEnter(Collider collide)
    {
        if (collide.gameObject.tag == "Tubo") //CHANGE TO TUBO SCRIPT WHEN IT IS AVALABLE
        {
            GrateHitbox.SetActive(true);
        }
    }

    void OnTriggerExit(Collider collide)
    {
        GrateHitbox.SetActive(false);
    }

}
