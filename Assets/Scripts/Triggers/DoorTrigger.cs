using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{

    public GameObject door;

    public string animationStateName;

    private Animator doorAnimation;

    void Start()
    {
        doorAnimation = door.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        doorAnimation.Play(animationStateName);
        gameObject.SetActive(false);
    }
}
