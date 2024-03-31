using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : MonoBehaviour
{
    //Setting used for determining if this wall is breakable by select robots
    public enum RobotsWhichCanDestroyWall
    {
        Hunter,
        Brutus,
        HunterAndBrutus
    }
    public RobotsWhichCanDestroyWall robotsWhichCanDestroyWallOption = RobotsWhichCanDestroyWall.HunterAndBrutus;

    public GameObject Object, BrokenObject, Decal;
    public Rigidbody[] ObjectShards;
    public float Force = 1000f;
    public float disabletime = 5;
    private float timer;
    private bool wallbreak;
    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        if (Object)
        Object.SetActive(true);
        if(BrokenObject)
        BrokenObject.SetActive(false);
        if(Decal)
        Decal.SetActive(true);
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (wallbreak && BrokenObject != null)
        {
            wallbreak = false;
            Object.SetActive(false);
            BrokenObject.SetActive(true);
            Decal.SetActive(false);
            timer = disabletime;
            transform.GetComponent<Collider>().enabled = false;
            foreach (Rigidbody shard in ObjectShards)
            {
                //shard.AddExplosionForce(Force, shard.transform.position, 4f, 5, ForceMode.Impulse);
                shard.AddForce(transform.forward * Force, ForceMode.Force);
            }
        }
        
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            if (BrokenObject && BrokenObject.activeInHierarchy)
            {
                foreach (Rigidbody shard in ObjectShards)
                {
                    shard.gameObject.SetActive(false);
                }
            }
        }
    }

    public void BreakWall()
    {
        if (!BrokenObject)
        {
            Destroy(gameObject);
        }
        else
        {
            wallbreak = true;
        }
    }
}
