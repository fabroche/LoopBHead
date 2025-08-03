using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{

    public float rotationSpeed = 0.5f;

    public GameObject onCollectEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed, 0); // Rotate the collectible object
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Logic to handle when the player collects the item
            UnityEngine.Debug.Log("Collectible collected!");
            Destroy(gameObject); // Destroy the collectible object

            UnityEngine.Debug.Log("Collectible VFX Spawned!");
            Instantiate(onCollectEffect, transform.position, transform.rotation); // Spawn the collectible effect

        }
    }
}
