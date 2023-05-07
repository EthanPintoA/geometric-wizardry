using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(SpellBoardReader))]

public class SpellBoard : MonoBehaviour
{
    [Header("Game Objects and Components")]
    public Transform playerCameraTransform;

    [Header("Variables")]
    [Tooltip("Distance between Spell Board and Camera")]
    public float distanceFromCamera;

    // The fixed distance and direction from the main camera.
    // It's used to keep the spell board from moving while drawing.
    private Vector3 fixedVectorFromCamera;

    private Renderer rendererComponent;
    private SpellBoardReader spellBoardReader;

    // Start is called before the first frame update
    private void Awake()
    {
        rendererComponent = GetComponent<Renderer>();
        spellBoardReader = GetComponent<SpellBoardReader>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        FollowCamera();
    }

    public void ToggleSpellBoard(InputAction.CallbackContext context)
    {
        if (!context.started) { return; }

        if (!rendererComponent.enabled)
        {
            fixedVectorFromCamera = transform.position - playerCameraTransform.position;
            rendererComponent.enabled = true;
        }
        else
        {
            spellBoardReader.Clear();
            rendererComponent.enabled = false;
        }
    }

    private void FollowCamera()
    {
        transform.position = playerCameraTransform.position;

        if (rendererComponent.enabled)
        {
            transform.position += fixedVectorFromCamera;
        }
        else
        {
            transform.position += playerCameraTransform.forward * distanceFromCamera;
            transform.rotation = playerCameraTransform.rotation;
        }
    }
}
