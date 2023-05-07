using UnityEngine;

public class Barrier : MonoBehaviour
{
    private float hitStopwatch = 0.0f;
    private readonly float hitAtTime = 3.0f;

    private readonly int breakAfterHitsNum = 2;
    private int numberOfTimesHit = 0;

    private void Update()
    {
        if (numberOfTimesHit > 0)
        {
            hitStopwatch += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Spell")) { return; }

        Destroy(other.gameObject);

        if (numberOfTimesHit == 0)
        {
            numberOfTimesHit += 1;
        }
        else if (hitStopwatch >= hitAtTime)
        {
            numberOfTimesHit += 1;

            if (numberOfTimesHit >= breakAfterHitsNum)
            {
                Destroy(gameObject);
            }
        }
    }
}
