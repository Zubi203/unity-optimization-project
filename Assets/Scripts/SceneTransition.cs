using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float transitionDuration = 0.5f;
    
    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(LoadNewScene(sceneName));
    }

    IEnumerator LoadNewScene(string sceneName)
    {
        animator?.SetTrigger("Transition");

        yield return new WaitForSeconds(transitionDuration);

        SceneManager.LoadScene(sceneName);
    }
}
