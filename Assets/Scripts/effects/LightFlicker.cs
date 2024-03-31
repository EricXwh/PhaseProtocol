using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    Light light;

    public float minspeed = 0.01f;

    public float maxspeed = 0.02f;

    public float minIntensity = 700f;

    public float maxIntensity = 900f;

    void Start()
    {
        light = GetComponent<Light>();
        StartCoroutine(run());
    }

    void Update()
    {

    }

    IEnumerator run()
    {
        while (true)
        {
            light.enabled = true;
            light.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(Random.Range(minspeed, maxspeed));
            light.enabled = true;
            yield return new WaitForSeconds(Random.Range(minspeed, maxspeed));
        }
    }
}