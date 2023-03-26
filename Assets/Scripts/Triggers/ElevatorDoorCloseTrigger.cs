using UnityEngine;

public class ElevatorDoorCloseTrigger : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject door;
    public GameObject player;

    [Header("Variables")]
    public string animationStateName;

    private Animator doorAnimation;

    private bool isPlayerInside = false;

    // Number of seconds player should be in elevator for;
    private float timeRemaining = 5;

    private void Awake()
    {
        doorAnimation = door.GetComponent<Animator>();
    }

    private void Update()
    {
        var elevatorGoingDown = IsDoorClosed() && isPlayerInside;
        if (!elevatorGoingDown) { return; }

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            return;
        }

        teleportPlayer();
        gameObject.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        var dot_product = Vector3.Dot(transform.forward, player.transform.position - transform.position);

        // Positive dot, greater than 0, product indicates player is past the door.
        if (dot_product <= 0) { return; }

        GetComponent<Collider>().isTrigger = false;
        isPlayerInside = true;
        doorAnimation.Play(animationStateName);
    }

    private bool IsDoorClosed()
    {
        var stateInfo = doorAnimation.GetCurrentAnimatorStateInfo(0);
        var normalizeTime = stateInfo.normalizedTime;
        var isName = stateInfo.IsName(animationStateName);

        return isName && normalizeTime >= 1;
    }

    private void teleportPlayer()
    {
        var playerController = player.GetComponent<CharacterController>();

        // Temporarily disabled playerController so it doesn't think we're
        // clipping though the ground. Otherwise it teleports us back up.
        playerController.enabled = false;
        player.transform.position += new Vector3(0, -10, 0);
        playerController.enabled = true;
    }
}
