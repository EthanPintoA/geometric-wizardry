using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("The speed of movement")]
    public float moveSpeed;
    [Tooltip("The speed of camera rotation")]
    public float lookSpeed;
    public float verticalLookLimit;
    [Tooltip("The downward force to apply to player while not grounded")]
    public float gravity;
    [Tooltip("The upward force to apply to player while not grounded")]
    public float jumpStrenth;
    [Tooltip("The max vertical force to apply to player while not grounded")]
    public float maxVelocity;


    [Header("Game Objects")]
    public Camera playerCamera;

    public RectTransform healthBar;

    [HideInInspector]
    public bool canMove = true;

    private Vector3 localMoveDirection = Vector3.zero;
    private Vector2 localRotation;

    private CharacterController characterController;

    private float health = 1.0f;
    private float healthBarMaxWidth;
    private float healthBarLeftPosX;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        var eulerAngles = transform.localRotation.eulerAngles;

        localRotation = new Vector2(eulerAngles.y, eulerAngles.x);

        healthBarMaxWidth = healthBar.sizeDelta.x;
        healthBarLeftPosX = healthBar.localPosition.x - (healthBar.rect.width / 2.0f);
    }

    private void Update()
    {
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

        if (!canMove) { return; }

        // Apply rotation
        playerCamera.transform.localRotation = Quaternion.Euler(localRotation.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, localRotation.x, 0);
    }

    void FixedUpdate()
    {
        // Apply movement
        var worldMoveDirection = transform.TransformDirection(localMoveDirection);
        characterController.Move(worldMoveDirection * (moveSpeed * Time.fixedDeltaTime));

        // Apply Gravity
        // Check if grounded and not jumping
        if (characterController.isGrounded && localMoveDirection.y < 0)
        {
            // -1 and not 0 because physics engine pushs object outside floor.
            // You need to push object into floor so `isGrounded` is true the
            // next game update.
            localMoveDirection.y = -1;
        }
        else
        {
            localMoveDirection.y -= gravity * Time.fixedDeltaTime;
            localMoveDirection.y = Mathf.Clamp(localMoveDirection.y, -maxVelocity, maxVelocity);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Spell")) { return; }

        health -= collision.gameObject.GetComponent<DamageDealer>().damage;
        RescaleHealthBar();

        if (health <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Debug.Log($"Loading Game Over Scene");
            SceneManager.LoadScene("Scenes/GameOver");
        }
    }

    private void RescaleHealthBar()
    {
        healthBar.sizeDelta = new Vector2(healthBarMaxWidth * health, healthBar.sizeDelta.y);

        var newManaBarLocalPos = healthBar.localPosition;
        newManaBarLocalPos.x = healthBarLeftPosX + (healthBar.sizeDelta.x / 2.0f);
        healthBar.localPosition = newManaBarLocalPos;
    }

    public void Move(InputAction.CallbackContext context)
    {
        // `input` is already normalized
        var input = context.ReadValue<Vector2>();
        localMoveDirection.x = input.x;
        localMoveDirection.z = input.y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started || !characterController.isGrounded) { return; }

        localMoveDirection.y += jumpStrenth;
    }

    public void Rotate(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();

        localRotation.y -= input.y * lookSpeed;
        localRotation.y = Mathf.Clamp(localRotation.y, -verticalLookLimit, verticalLookLimit);

        localRotation.x += input.x * lookSpeed;
    }
}
