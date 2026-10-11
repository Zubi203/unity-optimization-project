using UnityEngine;

public class TitleScreen : MonoBehaviour
{

    [SerializeField] private SceneTransition sceneLoader;
    public void OnPlayButtonPressed()
    {
        if (sceneLoader == null){return;}

        sceneLoader.TransitionToScene("Main");
    }

    public void OnInfoButtonPressed()
    {
        
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
}
