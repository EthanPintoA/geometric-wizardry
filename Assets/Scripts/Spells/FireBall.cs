using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed;
    public float travelDistance;

    private void Start()
    {
        var rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        // Travel's `travelDistance` before destroyed
        Destroy(gameObject, travelDistance / speed);

    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
