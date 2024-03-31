using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BluePrintCycle : MonoBehaviour
{
    [SerializeField] private Image blueprints;
    [SerializeField] private Sprite[] blueprintImages;

    private int cycleCount = 0;

    private Vector3 origPos;
    private Vector3 origScale;

    // Start is called before the first frame update
    void Start()
    {
        origPos = blueprints.transform.position;
        origScale = blueprints.transform.localScale;
        StartCoroutine(cycle());
        Debug.Log(cycleCount);
    }

    // Update is called once per frame
    void Update()
    {
        blueprints.transform.Translate(new Vector3(-5f, 5f, 0) * Time.deltaTime);
        blueprints.transform.localScale += (new Vector3(.002f, .002f, .002f) * Time.deltaTime);
    }

    //https://answers.unity.com/questions/225438/slowly-fades-from-opaque-to-alpha.html
    IEnumerator FadeTo(float aValue, float aTime)
    {
        float alpha = blueprints.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            blueprints.color = newColor;
            yield return null;
        }
    }

    IEnumerator cycle() {
        yield return new WaitForSeconds(10f);
        StartCoroutine(FadeTo(0, 5f));
        yield return new WaitForSeconds(5f);
        blueprints.transform.localScale = origScale;
        blueprints.transform.position = origPos;
        StartCoroutine(FadeTo(1, 2f));
        StartCoroutine(cycle());

        cycleCount++;

        if (cycleCount == blueprintImages.Length)
        {
            cycleCount = 0;
        }

        blueprints.sprite = blueprintImages[cycleCount];
        Debug.Log(cycleCount);
    }
}
