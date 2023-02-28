using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpellBoardReader : MonoBehaviour
{
    public Transform cameraTransform;
    public GameObject vertexPrefab;
    [Tooltip("Distance between Spell Board and Camera")]
    public float distanceFromCamera;
    [Tooltip("Minimum distance from first and last vertex")]
    public float minEndVertexDistance;
    [Tooltip("Minimum angle difference between vertexes and `incantations`")]
    public float minDegreesOfFreedom;

    // The spell names and their formulas
    private enum Spells : byte
    {
        Fireball,
        Barrier,
        Meteor,
    }
    // The formula is the angle a point must be from the previous point.
    // First angle is the angle from point 0 to 1, etc.
    private readonly List<float>[] incantations = {
        new List<float>{-45, 180, 45}, // Triangle
        new List<float>{0, -90, 180, 90} // Square

    };

    // Check if a spell is being casted
    private bool playerCasting = false;

    // The pitch and yaw angles of the spell currently being casted.
    private readonly List<Vector2> currentSpell = new();
    private readonly List<GameObject> vertexes = new();

    private Vector3 fixedPositionFromCamera;

    void Update()
    {
        FollowCamera();

        if (Input.GetMouseButtonDown(1))
        {
            ToggleCasting();
        }

        if (playerCasting && Input.GetMouseButtonDown(0))
        {
            AddToCurrentSpell();
            if (IsSpellComplete())
            {
                var spell = GetSpell();

                ToggleCasting();
                Debug.Log(spell);
            }
        }

    }

    private void FollowCamera()
    {
        transform.position = cameraTransform.position;

        if (playerCasting)
        {
            transform.position += fixedPositionFromCamera;
        }
        else
        {
            transform.position += cameraTransform.forward * distanceFromCamera;
            transform.rotation = cameraTransform.rotation;
        }
    }

    private void ToggleCasting()
    {
        playerCasting = !playerCasting;
        Renderer objectRenderer = GetComponent<Renderer>();

        // Toggled to
        if (playerCasting)
        {
            fixedPositionFromCamera = transform.position - cameraTransform.position;
            objectRenderer.enabled = true;
        }
        else
        {
            currentSpell.Clear();
            vertexes.ForEach(Destroy);
            vertexes.Clear();

            objectRenderer.enabled = false;
        }

    }

    private bool AddToCurrentSpell()
    {
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

        return didHit;
    }

    private bool IsSpellComplete()
    {
        // Spells casted need to have more than one vertex and
        // the first and last vertex should be relatively the same position.
        return (
            currentSpell.Count > 1 &&
            Vector2.Distance(currentSpell[0], currentSpell[^1]) < minEndVertexDistance
        );
    }

    private Spells? GetSpell()
    {
        // // Log the `currentSpell` list
        // Debug.Log(System.String.Join(", ", currentSpell.ConvertAll(v => v.ToString()).ToArray()));

        var possibleSpells = incantations
            .AsEnumerable()
            // Map to get index
            .Select((l, i) => (l, i))
            // Filter incantations with incorrect length
            .Where(t => t.l.Count == currentSpell.Count - 1)
            .Where(t => ValidateCurrentSpell(t.l));

        return possibleSpells.Count() == 0 ? null : (Spells)possibleSpells.First().i;
    }

    [System.Obsolete("This method is deprecated in favor of `getSpell`")]
    private Spells? GetSpellAlt()
    {
        for (int i = 0; i < incantations.Length; i++)
        {
            var incantation = incantations[i];

            // Filter incantations with incorrect length
            if (incantation.Count != currentSpell.Count - 1) { continue; }
            if (ValidateCurrentSpell(incantation))
            {
                return (Spells)i;
            }
        }
        return null;
    }

    private bool ValidateCurrentSpell(List<float> incantation)
    {
        return currentSpell
            .Zip(currentSpell.Skip(1), (v1, v2) => (v: v1, vNxt: v2))
            // Map to current spell angles
            .Select(t => Vector2.SignedAngle(Vector2.right, t.vNxt - t.v))
            .Zip(incantation, (a, e) => (actualAngle: a, expectedAngle: e))
            // Map to angle difference
            .Select(t => Mathf.DeltaAngle(t.actualAngle, t.expectedAngle))
            .All(angleDiff => Mathf.Abs(angleDiff) <= minDegreesOfFreedom);
    }

    [System.Obsolete("This method is deprecated in favor of `validateCurrentSpell`")]
    private bool ValidateCurrentSpellAlt(List<float> incantation)
    {
        for (int i = 0; i < incantation.Count; i++)
        {
            var currentSpellAngle = Vector2.SignedAngle(
                Vector2.right,
                currentSpell[i + 1] - currentSpell[i]
            );

            var angleDiff = Mathf.DeltaAngle(currentSpellAngle, incantation[i]);

            if (Mathf.Abs(angleDiff) > minDegreesOfFreedom)
            {
                return false;
            }
        }

        return true;
    }
}
