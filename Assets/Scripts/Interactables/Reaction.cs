using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaction : MonoBehaviour
{

    [SerializeField]
    private GameObject destroyGate;
    public virtual void objectReaction() {
        Destroy(destroyGate); 
    
    }

    public virtual void objectReactionLeave(){} //For Pressure plates, called when getting off.
}
