using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject player;
    public GameObject spell;

    [Header("Variables")]
    public float walkRadius;

    private NavMeshAgent agent;

    private float health = 1.0f;

    private float launchSpellStopwatch = 0.0f;
    private readonly float launchSpellAtTime = 5f;

    // Prevent Agent from getting stuck
    private float timeStuck = 0.0f;
    private readonly float stuckTimeThreshold = 3.0f;

    private Vector3 lastPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;

        launchSpellStopwatch = Random.Range(0.0f, launchSpellAtTime);
    }

    void Update()
    {
        // Barely moved
        if (Vector3.Distance(agent.transform.position, lastPosition) < 0.01f)
        {
            timeStuck += Time.deltaTime;
        }
        else
        {
            timeStuck = 0;
        }

        if (agent.remainingDistance <= agent.stoppingDistance || timeStuck > stuckTimeThreshold)
        {
            agent.SetDestination(RandomPositionWithin2DRadius(walkRadius));
            timeStuck = 0;
        }

        if (launchSpellStopwatch >= launchSpellAtTime)
        {
            SummonSpell(player.transform.position - transform.position);
            launchSpellStopwatch = 0;
        }
        launchSpellStopwatch += Time.deltaTime;
    }

    /// <summary>
    /// Returns a random position along the floor
    /// </summary>
    private Vector3 RandomPositionWithin2DRadius(float radius)
    {
        var randomRelativePos = new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));

        return transform.position - randomRelativePos;
    }

    private void SummonSpell(Vector3 direction)
    {
        var spellPosition = transform.position + (direction.normalized * 3.0f);

        Instantiate(spell, spellPosition, Quaternion.LookRotation(direction));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Spell")) { return; }

        health -= collision.gameObject.GetComponent<DamageDealer>().damage;

        if (health <= 0)
        {
            Destroy(gameObject);
            enabled = false;
        }
    }
}
