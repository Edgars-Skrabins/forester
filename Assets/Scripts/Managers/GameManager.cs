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

    [SerializeField] private bool canMove;
    [SerializeField] private InputActionReference pauseAction;
    private void Start()
    {
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
        gameState = GameState.Cutscene;
        uiManager.HideAll();
        StartCoroutine(CutsceneCoroutine());
    }

    public void PauseGame()
    {
        if (gameState != GameState.Playing) return;

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}