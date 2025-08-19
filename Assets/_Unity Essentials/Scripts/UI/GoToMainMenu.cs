using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GoToMainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddButtonListener(this.GetComponent<Button>(), GoToMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    
    private void AddButtonListener(Button button, UnityAction function)
    {
        if (button != null)
        {
            button.onClick.AddListener(function);
        }
    }
}
