using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    public string nextScene;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Loading {nextScene}");
        SceneManager.LoadSceneAsync(nextScene);
    }
}
