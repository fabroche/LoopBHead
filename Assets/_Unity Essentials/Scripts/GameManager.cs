using System;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Over UI Elements")] public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    [Header("You Win UI Elements")] public GameObject youWinPanel;
    public TextMeshProUGUI youWinText;
    
    [Header("Pause UI Elements")] public GameObject pausePanel;

    [Header("Buttons")] public Button restartButton;
    public Button menuButton;
    public Button nextLevelButton;
    public Button quitButton;

    public KeyCode pauseKey = KeyCode.Escape;
    public KeyCode restartKey = KeyCode.R;

    private bool _isGameoverActive = false;
    private bool _isYouWinActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        Instance = this;
        // DontDestroyOnLoad(this);
    }

    void Start()
    {
        SetPanelVisibility(gameOverPanel, false);

        SetPanelVisibility(youWinPanel, false);
        
        SetPanelVisibility(pausePanel, false);

        AddButtonListener(restartButton, RestartScene);

        AddButtonListener(menuButton, GoToMenu);

        AddButtonListener(nextLevelButton, NextLevel);
    }

    void Update()
    {
        TogglePausePanel();
    }

    private void TogglePausePanel()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPanelVisibility(pausePanel, !GetPanelVisibility(pausePanel));
            Time.timeScale = GetPanelVisibility(pausePanel) ? Time.timeScale = 0f : Time.timeScale = 1f;
        }
    }

    private void AddButtonListener(Button button, UnityAction function)
    {
        if (button != null)
        {
            button.onClick.AddListener(function);
        }
    }

    public void SetPanelVisibility(GameObject panel, bool isActive)
    {
        if (panel != null)
        {
            panel.SetActive(isActive);
        }
    }

    public bool GetPanelVisibility(GameObject panel)
    {
        return panel.activeSelf;
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void NextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GameOver()
    {
        if (_isGameoverActive) return;

        _isGameoverActive = true;

        SetPanelVisibility(gameOverPanel, true);
        Time.timeScale = 0f;
    }

    public void YouWin()
    {
        if (_isYouWinActive) return;

        _isYouWinActive = true;

        SetPanelVisibility(youWinPanel, true);
        Time.timeScale = 0f;
    }
}