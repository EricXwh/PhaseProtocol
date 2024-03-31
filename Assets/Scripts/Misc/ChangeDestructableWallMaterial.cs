using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Does not work rn */
public class ChangeDestructableWallMaterial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var destructableWalls = Object.FindObjectsOfType(typeof(DestructibleWall));
        Debug.Log("Num destructable walls = " + destructableWalls.Length);
        foreach (DestructibleWall destructableWall in destructableWalls)
        {
            var wallRenderer = destructableWall.GetComponent<Renderer>();
            wallRenderer.material.SetColor("_Color", Color.blue);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
