using UnityEngine;

public class DayNightCycle : MonoBehaviour
{

    public float sunRotationSpeed = 0.5f; // Speed of the sun's rotation

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(sunRotationSpeed, 0, 0); // Rotate the object around the X-axis
    }
}
