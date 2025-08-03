using System.Diagnostics;
using UnityEngine;

public class TeleportZone : MonoBehaviour
{
    

    [Header("Teleport Settings")]
    [SerializeField] private Transform destination;
    [SerializeField] private Vector3 destinationOffset = Vector3.zero;
    [SerializeField] private bool teleportOnEnter = true;
    [SerializeField] private KeyCode teleportKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    [Header("Effects")]
    [SerializeField] private bool showTeleportMessage = true;
    [SerializeField] private string teleportMessage = "Presiona E para teletransportarte";

    private bool playerInZone = false;
    private GameObject currentPlayer;

    void Start()
    {
        // Asegurar que tiene Collider2D con Trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        // Si el jugador está en la zona y presiona la tecla
        if (playerInZone && !teleportOnEnter && Input.GetKeyDown(teleportKey))
        {
            TeleportPlayer();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            currentPlayer = other.gameObject;
            playerInZone = true;

            if (teleportOnEnter)
            {
                TeleportPlayer();
            }
            else if (showTeleportMessage)
            {
                UnityEngine.Debug.Log(teleportMessage);
                // Aquí podrías mostrar UI
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInZone = false;
            currentPlayer = null;
        }
    }

    void TeleportPlayer()
    {
        if (currentPlayer == null || destination == null) return;

        Vector3 teleportPosition = destination.position + destinationOffset;

        // Parar movimiento
        Rigidbody2D rb = currentPlayer.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Teletransportar
        currentPlayer.transform.position = teleportPosition;

        UnityEngine.Debug.Log($"¡Teletransportado a {destination.name}!");
    }

    // Visualización en editor
    void OnDrawGizmosSelected()
    {
        // Dibujar zona de teletransporte
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, transform.localScale);

        // Dibujar línea al destino
        if (destination != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, destination.position + destinationOffset);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(destination.position + destinationOffset, 0.5f);
        }
    }
}