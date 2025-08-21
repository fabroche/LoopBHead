using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseGameController : MonoBehaviour
{
    private bool _isPaused = false;
    void Start()
    {
        AddButtonListener(this.GetComponent<Button>(), TogglePause);
        _isPaused = GameManager.Instance.GetPanelVisibility(GameManager.Instance.pausePanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void TogglePause()
    {
        _isPaused = !_isPaused;
        if (_isPaused)
        {
            Time.timeScale = 0f; // Pausar el juego
            GameManager.Instance.SetPanelVisibility(GameManager.Instance.pausePanel, true);
        }
        else
        {
            Time.timeScale = 1f; // Reanudar el juego
            GameManager.Instance.SetPanelVisibility(GameManager.Instance.pausePanel, false);
        }
    }
    
    private void AddButtonListener(Button button, UnityAction function)
    {
        if (button != null)
        {
            button.onClick.AddListener(function);
        }
    }
}