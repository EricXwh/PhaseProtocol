using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Conveyor : MonoBehaviour
{
    [SerializeField]
    public float speed;
    [SerializeField]
    private Vector3 direction;
    [SerializeField]
    private GameObject belt;
    // Start is called before the first frame update
    void Start()
    {
        belt = gameObject;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<CharacterController>() != null)
        {
            CharacterController cController = other.GetComponent<CharacterController>();

            cController.Move(direction * speed * Time.deltaTime);
        }
        else
        {
            other.transform.position = Vector3.MoveTowards(
                                                        other.transform.position,
                                                        other.transform.position + direction,
                                                        speed * Time.deltaTime
                                                    );
        }
    }
}
