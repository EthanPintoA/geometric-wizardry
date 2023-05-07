using UnityEngine;

public class FloorEnemiesManager : MonoBehaviour
{
    public GameObject floorBarrier;

    void Update()
    {
        if (transform.childCount == 0)
        {
            floorBarrier.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
