using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private SceneTransition sceneLoader;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject toolkitPanel;
    
    public void OnStatsToggleButtonPressed()
    {
        if (statsPanel == null){return;}

        statsPanel.SetActive(!statsPanel.activeInHierarchy);
    }

    public void OnToolkitToggleButtonPressed()
    {
        if (toolkitPanel == null){return;}

        toolkitPanel.SetActive(!toolkitPanel.activeInHierarchy);
    }

    public void OnMainMenuButtonPressed()
    {
        if (sceneLoader == null){return;}

        sceneLoader.TransitionToScene("TitleScreen");
    }
}
