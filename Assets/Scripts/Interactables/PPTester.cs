using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPTester : Reaction
{
    //Testing Script for pressureplates

    private bool spiiin = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spiiin == true) {
            transform.Rotate(new Vector3(5, 5, 2));
        }
    }

    public override void objectReaction() {
        spiiin = true;
    }

    public override void objectReactionLeave()
    {
        spiiin = false;
    }

}
