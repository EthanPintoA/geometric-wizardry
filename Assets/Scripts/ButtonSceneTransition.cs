using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneTransition : MonoBehaviour
{
    public string nextScene;

    public void OnClick()
    {
        Debug.Log($"Loading {nextScene}");
        SceneManager.LoadSceneAsync(nextScene);
    }
}