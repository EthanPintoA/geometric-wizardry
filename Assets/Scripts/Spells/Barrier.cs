using UnityEngine;

public class Barrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Spell"))
        {
            Destroy(other.gameObject);
        }
    }
}
