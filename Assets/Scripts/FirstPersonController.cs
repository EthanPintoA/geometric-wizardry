using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("The speed of movement")]
    public float moveSpeed;
    [Tooltip("The speed of camera rotation")]
    public float lookSpeed;
    public float verticalLookLimit;

    [Header("Game Objects")]
    public Camera playerCamera;

    [HideInInspector]
    public bool canMove = true;

    // Camera's current rotation around x-axis and y-axis
    private float verticalRotation = 0;
    private float horizontalRotation = 0;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (canMove) { RotatePlayer(); }

        // Toggle cursor lock & visibility
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            canMove = !canMove;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    void FixedUpdate()
    {
        if (canMove) { MovePlayer(); }
    }

    private void MovePlayer()
    {
        var horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.fixedDeltaTime;
        var verticalMovement = Input.GetAxis("Vertical") * moveSpeed * Time.fixedDeltaTime;

        var motion = transform.TransformDirection(horizontalMovement, 0, verticalMovement);
        characterController.Move(motion);
    }

    private void RotatePlayer()
    {
        verticalRotation -= Input.GetAxis("Mouse Y") * lookSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        horizontalRotation += Input.GetAxis("Mouse X") * lookSpeed;
        transform.localRotation = Quaternion.Euler(0, horizontalRotation, 0);
    }
}
