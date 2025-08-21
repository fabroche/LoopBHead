using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartScene : MonoBehaviour
{
    void Start()
    {
        AddButtonListener(this.GetComponent<Button>(), HandleRestartScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void HandleRestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void AddButtonListener(Button button, UnityAction function)
    {
        if (button != null)
        {
            button.onClick.AddListener(function);
        }
    }
}
