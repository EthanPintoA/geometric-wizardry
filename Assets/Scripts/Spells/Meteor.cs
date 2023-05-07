using UnityEngine;

[RequireComponent(typeof(DamageDealer))]
public class Meteor : MonoBehaviour
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
        if (collision.gameObject.CompareTag("Spell"))
        {
            if (collision.gameObject.GetComponent<DamageDealer>().damage >=gameObject.GetComponent<DamageDealer>().damage)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
