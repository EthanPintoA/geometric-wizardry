using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellBoardReader : MonoBehaviour
{
    [Header("Game Objects and Components")]
    public Transform playerCameraTransform;

    [Tooltip("The vertex point displayed on the board")]
    public GameObject vertexPrefab;

    public GameObject fireballPrefab;
    public GameObject barrierPrefab;
    public GameObject meteorPrefab;

    public RectTransform manaBar;

    [Header("Variables")]
    [Tooltip("Minimum distance from first and last vertex")]
    public float minEndVertexDistance;
    [Tooltip("Minimum angle difference between vertexes and `invocations`")]
    public float minDegreesOfFreedom;

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
        new List<float>{0, -90, 180, 90}, // Square
        new List<float>{-45, -135, 135, 45 } // Diamond
    };

    // The pitch and yaw angles of the spell currently being casted.
    private readonly List<Vector2> currentSpell = new();
    private readonly List<GameObject> vertexes = new();

    private Renderer rendererComponent;

    // Mana related stuff
    private float mana = 1.0f;
    private readonly float manaRegenPerSec = 0.05f;
    private float manaBarMaxWidth;
    private float manaBarLeftPosX;
    private readonly float fireballManaCost = 0.5f;
    private readonly float meteorManaCost = 0.80f;
    private readonly float barrierManaCost = 0.65f;


    private void Start()
    {
        rendererComponent = GetComponent<Renderer>();

        manaBarMaxWidth = manaBar.sizeDelta.x;
        manaBarLeftPosX = manaBar.localPosition.x - (manaBar.rect.width / 2.0f);
    }

    void Update()
    {
        if (rendererComponent.enabled && IsSpellComplete())
        {
            var spell = GetSpell();

            if (spell is not null)
            {
                if (IsEnoughManaForSpell((Spell)spell))
                {
                    SummonSpell((Spell)spell);
                }

                Clear();
                rendererComponent.enabled = false;
            }
        }

        mana += manaRegenPerSec * Time.deltaTime;
        mana = Mathf.Clamp(mana, 0, 1);
        RescaleManaBar();
    }

    public void PlaceVertex(InputAction.CallbackContext context)
    {
        if (!context.started || !rendererComponent.enabled) { return; }

        var didHit = Physics.Raycast(
            playerCameraTransform.position,
            playerCameraTransform.forward,
            out RaycastHit hitInfo,
            Mathf.Infinity,
            1 << 6
        );

        if (!didHit) { return; }

        var pointOnBoard = transform.InverseTransformPoint(hitInfo.point);
        pointOnBoard.Scale(transform.localScale);
        currentSpell.Add(pointOnBoard);

        var vertexPoint = transform.InverseTransformPoint(hitInfo.point);
        vertexPoint.z = 0;

        var vertexObject = Instantiate(vertexPrefab, transform);
        vertexObject.transform.localPosition = vertexPoint;

        vertexes.Add(vertexObject);
    }

    public void Clear()
    {
        currentSpell.Clear();

        vertexes.ForEach(Destroy);
        vertexes.Clear();
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
            var fireballObject = Instantiate(fireballPrefab, transform.position, playerCameraTransform.rotation);
            fireballObject.transform.rotation = playerCameraTransform.rotation;

            mana -= fireballManaCost;

            Debug.Log("Summoned Spell: Fireball");
        }
        else if (spell == Spell.Barrier)
        {
            Instantiate(barrierPrefab, transform.position, playerCameraTransform.rotation);

            mana -= barrierManaCost;

            Debug.Log("Summoned Spell: Barrier");
        }
        else if (spell == Spell.Meteor)
        {
            Instantiate(meteorPrefab, transform.position, playerCameraTransform.rotation);

            mana -= meteorManaCost;

            Debug.Log("Summoned Spell: Meteor");
        }
        else
        {
            Debug.LogError($"Spell {spell} doesn't have an instantiation");
        }

        RescaleManaBar();
    }

    private void RescaleManaBar()
    {
        manaBar.sizeDelta = new Vector2(manaBarMaxWidth * mana, manaBar.sizeDelta.y);

        var newManaBarLocalPos = manaBar.localPosition;
        newManaBarLocalPos.x = manaBarLeftPosX + (manaBar.sizeDelta.x / 2.0f);
        manaBar.localPosition = newManaBarLocalPos;
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

    private bool IsEnoughManaForSpell(Spell spell)
    {
        if (spell == Spell.Fireball)
        {
            return mana >= fireballManaCost;
        }
        else if (spell == Spell.Barrier)
        {
            return mana >= barrierManaCost;
        }
        else if (spell == Spell.Meteor)
        {
            return mana >= meteorManaCost;
        }
        else
        {
            Debug.LogError($"Spell {spell} doesn't have an mana cost");
            return false;
        }
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
