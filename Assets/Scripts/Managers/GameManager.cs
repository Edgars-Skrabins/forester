using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public enum GameState
{
    Menu,
    Playing,
    Paused,
    Cutscene
}

public class GameManager : Singleton<GameManager>
{
    public GameState gameState = GameState.Menu;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject titleCam;
    [SerializeField] private InputActionReference pauseAction;
    
    private void Start()
    {
        AudioManager.Instance.PlaySound("BGM_MainMenu");
        AudioManager.Instance.PlaySound("BGM_Ambience_0");
        uiManager.ShowMainMenu();
    }

    private void Update()
    {
        if (gameState == GameState.Playing && pauseAction.action.WasPerformedThisFrame())
        {
            PauseGame();
        }
        else if (gameState == GameState.Paused && pauseAction.action.WasPerformedThisFrame())
        {
            ResumeGame();
        }
    }

    public void StartGame()
    {
        titleCam.SetActive(false);
        gameState = GameState.Cutscene;
        uiManager.HideAll();
        StartCoroutine(CutsceneCoroutine());
        AudioManager.Instance.PlaySound("UISFX_Play");
        AudioManager.Instance.StopSound("BGM_MainMenu");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        if (gameState != GameState.Playing) return;

        AudioManager.Instance.PlaySound("UISFX_Back");
        gameState = GameState.Paused;
        Time.timeScale = 0f;
        uiManager.ShowPauseMenu();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (gameState != GameState.Paused) return;

        gameState = GameState.Playing;
        AudioManager.Instance.PlaySound("UISFX_Play");
        Time.timeScale = 1f;
        uiManager.ShowHUD();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private IEnumerator CutsceneCoroutine()
    {
        // Placeholder for cutscene logic
        yield return null; 
        
        gameState = GameState.Playing;
        uiManager.ShowHUD();
        player.SetActive(true);
        
    }

    public UIManager GetUIManager()
    {
        return uiManager;
    }
    
    public void QuitApplication()
    {
        Application.Quit();
    }
}