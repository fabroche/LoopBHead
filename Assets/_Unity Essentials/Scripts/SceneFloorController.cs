using UnityEngine;

public class SceneFloorController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Logic to handle when the player collides with the floor
         //   UnityEngine.Debug.Log("Player has collided with the floor!");
        }
    }
}
