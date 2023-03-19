using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{

    public GameObject door;

    private Animator doorAnimation;

    // Start is called before the first frame update
    void Start()
    {
        doorAnimation = door.GetComponent<Animator>();
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        doorAnimation.Play("Base Layer.Door Animation");
    }
}
