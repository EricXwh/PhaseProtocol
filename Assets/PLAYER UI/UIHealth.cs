using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    [SerializeField] public Text HealthText;

    // Start is called before the first frame update
    void Start()
    {
        HealthText.text = GameManager.playerHP.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        HealthText.text = GameManager.playerHP.ToString();
    }
}
