using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellBoardReader : MonoBehaviour
{
    [Header("Game Objects and Components")]
    public Transform cameraTransform;
    [Tooltip("The vertex point displayed on the board")]
    public GameObject vertexPrefab;
    public GameObject fireballPrefab;
    public GameObject barrierPrefab;
    public GameObject meteorPrefab;

    [Header("Variables")]
    [Tooltip("Distance between Spell Board and Camera")]
    public float distanceFromCamera;
    [Tooltip("Minimum distance from first and last vertex")]
    public float minEndVertexDistance;
    [Tooltip("Minimum angle difference between vertexes and `invocations`")]
    public float minDegreesOfFreedom;

    [HideInInspector]
    // Check if spell board is visible
    public bool playerCasting = false;

    // The spell names and their formulas
    private enum Spell : byte
    {
        Fireball,
        Barrier,
        Meteor,
    }
    // The formula is the angle a point must be from the previous point.
    // First angle is the angle from point 0 to 1, etc.
    private readonly List<float>[] invocations = {
        new List<float>{-45, 180, 45}, // Triangle
        new List<float>{0, -90, 180, 90} // Square

    };

    // The pitch and yaw angles of the spell currently being casted.
    private readonly List<Vector2> currentSpell = new();
    private readonly List<GameObject> vertexes = new();

    // The fixed distance and direction from the main camera.
    // It's used to keep the spell board from moving while drawing.
    private Vector3 fixedVectorFromCamera;

    private Renderer rendererComponent;

    private void Awake()
    {
        rendererComponent = GetComponent<Renderer>();
    }

    void Update()
    {
        FollowCamera();

        if (playerCasting && IsSpellComplete())
        {
            var spell = GetSpell();

            if (spell is not null)
            {
                SummonSpell((Spell)spell);
            }
        }
    }

    private void FollowCamera()
    {
        transform.position = cameraTransform.position;

        if (playerCasting)
        {
            transform.position += fixedVectorFromCamera;
        }
        else
        {
            transform.position += cameraTransform.forward * distanceFromCamera;
            transform.rotation = cameraTransform.rotation;
        }
    }

    public void ToggleSpellBoard(InputAction.CallbackContext context)
    {
        if (!context.started) { return; }

        if (!playerCasting)
        {
            EnableBoard();
        }
        else
        {
            DisableBoard();
        }
    }

    public void PlaceVertex(InputAction.CallbackContext context)
    {
        if (!context.started || !playerCasting) { return; }

        RaycastHit hitInfo;
        var didHit = Physics.Raycast(
            cameraTransform.position,
            cameraTransform.forward,
            out hitInfo,
            Mathf.Infinity,
            1 << 6
        );

        if (didHit)
        {
            var pointOnBoard = transform.InverseTransformPoint(hitInfo.point);
            pointOnBoard = Vector3.Scale(pointOnBoard, transform.localScale);

            var vertexPoint = transform.InverseTransformPoint(hitInfo.point);
            vertexPoint.z = 0;

            currentSpell.Add(pointOnBoard);

            var vertexObject = Instantiate(vertexPrefab, transform);
            vertexObject.transform.localPosition = vertexPoint;

            vertexes.Add(vertexObject);
        }
    }

    private void EnableBoard()
    {
        fixedVectorFromCamera = transform.position - cameraTransform.position;

        playerCasting = true;
        rendererComponent.enabled = true;
    }

    private void DisableBoard()
    {
        currentSpell.Clear();

        vertexes.ForEach(Destroy);
        vertexes.Clear();

        playerCasting = false;
        rendererComponent.enabled = false;
    }

    /// <returns>
    /// If the last vertex is on the first vertex.
    /// </returns>
    private bool IsSpellComplete()
    {
        // Spell casted need to have more than one vertex and
        // the first and last vertex should be relatively the same position.
        return (
            currentSpell.Count > 1 &&
            Vector2.Distance(currentSpell[0], currentSpell[^1]) < minEndVertexDistance
        );
    }

    private void SummonSpell(Spell spell)
    {
        if (spell == Spell.Fireball)
        {
            var fireballObject = Instantiate(fireballPrefab, transform.position, cameraTransform.rotation);
            fireballObject.transform.rotation = cameraTransform.rotation;

            Debug.Log("Summoned Spell: Fireball");
        }
        else if (spell == Spell.Barrier)
        {
            Instantiate(barrierPrefab, transform.position, cameraTransform.rotation);

            Debug.Log("Summoned Spell: Barrier");
        }
        else if (spell == Spell.Meteor)
        {
            Instantiate(meteorPrefab, transform.position, cameraTransform.rotation);

            Debug.Log("Summoned Spell: Meteor");
        }
        else
        {
            Debug.LogError($"Spell {spell} doesn't have an instantiation");
        }

        DisableBoard();
    }

    private Spell? GetSpell()
    {
        var possibleSpells = invocations
            .AsEnumerable()
            // Map to get index
            .Select((l, i) => (l, i))
            // Filter invocations with incorrect length
            .Where(t => t.l.Count == currentSpell.Count - 1)
            .Where(t => ValidateCurrentSpell(t.l));

        return possibleSpells.Count() == 0 ? null : (Spell)possibleSpells.First().i;
    }

    private bool ValidateCurrentSpell(List<float> invocation)
    {
        return currentSpell
            .Zip(currentSpell.Skip(1), (v1, v2) => (v: v1, vNxt: v2))
            // Map to current spell angles
            .Select(t => Vector2.SignedAngle(Vector2.right, t.vNxt - t.v))
            .Zip(invocation, (a, e) => (actualAngle: a, expectedAngle: e))
            // Map to angle difference
            .Select(t => Mathf.DeltaAngle(t.actualAngle, t.expectedAngle))
            .All(angleDiff => Mathf.Abs(angleDiff) <= minDegreesOfFreedom);
    }

    [System.Obsolete("This method is deprecated in favor of `getSpell`")]
    private Spell? GetSpellAlt()
    {
        for (int i = 0; i < invocations.Length; i++)
        {
            var invocation = invocations[i];

            // Filter invocations with incorrect length
            if (invocation.Count != currentSpell.Count - 1) { continue; }
            if (ValidateCurrentSpell(invocation))
            {
                return (Spell)i;
            }
        }
        return null;
    }

    [System.Obsolete("This method is deprecated in favor of `validateCurrentSpell`")]
    private bool ValidateCurrentSpellAlt(List<float> invocation)
    {
        for (int i = 0; i < invocation.Count; i++)
        {
            var currentSpellAngle = Vector2.SignedAngle(
                Vector2.right,
                currentSpell[i + 1] - currentSpell[i]
            );

            var angleDiff = Mathf.DeltaAngle(currentSpellAngle, invocation[i]);

            if (Mathf.Abs(angleDiff) > minDegreesOfFreedom)
            {
                return false;
            }
        }

        return true;
    }
}
