using UnityEngine;

[System.Serializable]
public class UIManager
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private GameObject hudUI;
    
    public void ShowMainMenu()
    {
        HideAll();
        if (mainMenuUI != null)
            mainMenuUI.SetActive(true);
    }
    
    public void ShowPauseMenu()
    {
        HideAll();
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
    }
    
    public void ShowSettingsMenu()
    {
        HideAll();
        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(true);
    }
    
    public void ShowHUD()
    {
        HideAll();
        if (hudUI != null)
            hudUI.SetActive(true);
    }

    public void HideAll()
    {
        if (mainMenuUI != null)
            mainMenuUI.SetActive(false);
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);
        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false);
        if (hudUI != null)
            hudUI.SetActive(false);
    }
}