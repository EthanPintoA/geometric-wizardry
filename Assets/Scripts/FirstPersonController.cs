using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Tooltip("The speed of movement")]
    public float moveSpeed = 5.0f;

    [Tooltip("The speed of camera rotation")]
    public float lookSpeed = 5.0f;

    // Camera's current rotation around x-axis and y-axis
    private float verticalRotation = 0.0f;
    private float horizontalRotation = 0.0f;

    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        movePlayer();
        rotateCameraAndPlayer();

        // "Toggle cursor lock & visibility"
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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

    private void rotateCameraAndPlayer()
    {
        verticalRotation -= Input.GetAxis("Mouse Y") * lookSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, -90.0f, 90.0f);

        horizontalRotation += Input.GetAxis("Mouse X") * lookSpeed;

        transform.localRotation = Quaternion.Euler(verticalRotation, 0.0f, 0.0f);
        transform.parent.localRotation = Quaternion.Euler(0.0f, horizontalRotation, 0.0f);
    }

    private void movePlayer()
    {
        float horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float verticalMovement = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.parent.Translate(new Vector3(horizontalMovement, 0.0f, verticalMovement));
    }
}
