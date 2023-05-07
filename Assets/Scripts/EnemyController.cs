using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Variables")]
    public float walkRadius;

    private NavMeshAgent agent;

    // Prevent Agent from getting stuck
    private float timeStuck = 0.0f;
    private readonly float stuckTimeThreshold = 3.0f;

    private Vector3 lastPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastPosition = transform.position;
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
    }

    /// <summary>
    /// Returns a random position along the floor
    /// </summary>
    private Vector3 RandomPositionWithin2DRadius(float radius)
    {
        var randomRelativePos = new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));

        return transform.position - randomRelativePos;
    }
}
