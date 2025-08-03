using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible2D : MonoBehaviour
{

    [Header("Tags")]
    public string playerTag = "Player";

    [Header("Collectible Settings")]
    public float rotationSpeedX = 0;
    public float rotationSpeedY = 0;
    public float rotationSpeedZ = 0.5f;
    public GameObject onCollectEffect;

    [Header("Floating Animation")]
    public bool isFloating = true;
    public float floatHeight = 0.5f;
    public float floatSpeed = 2f;

    private Vector3 startPosition;

    void Start()
    {
        if (isFloating)
        {
            SetFloatingAnimation();
        }
    }

    void Update()
    {
        transform.Rotate(rotationSpeedX, rotationSpeedY, rotationSpeedZ);

        if (isFloating)
        {
            FloatingUpdate();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        
         // Check if the other object has a Player Tag
        if (other.gameObject.CompareTag(playerTag)) {
            
            // Destroy the collectible
            Destroy(gameObject);

            // Instantiate the particle effect
            Instantiate(onCollectEffect, transform.position, transform.rotation);
        }

        
    }

    void SetFloatingAnimation()
    {
        startPosition = transform.position;
    }

    void FloatingUpdate()
    {
        // Crear movimiento flotante usando seno
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

}


