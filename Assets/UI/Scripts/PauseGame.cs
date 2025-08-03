using UnityEngine;

public class PauseGame : MonoBehaviour
{

    public GameObject pauseMenu;

    public bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f; // Pausar el juego
            pauseMenu.SetActive(true); // Mostrar el menú de pausa
        }
        else
        {
            Time.timeScale = 1f; // Reanudar el juego
            pauseMenu.SetActive(false); // Ocultar el menú de pausa
        }
    }
}
