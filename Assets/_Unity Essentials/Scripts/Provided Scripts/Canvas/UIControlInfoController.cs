using TMPro;
using UnityEngine;

public class UIControlInfoController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public KeyCode Key = KeyCode.Tab;
    public GameObject textMeshProObject;
    public string actionName = "Time Jump";
    
    void OnEnable()
    {
        textMeshProObject.GetComponent<TextMeshProUGUI>().text = $"[{Key}] {actionName}";
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
