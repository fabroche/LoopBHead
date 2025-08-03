using System.Diagnostics;
using UnityEngine;

public class TeleportOnKey : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private Vector3 teleportPosition = Vector3.zero;
    [SerializeField] private Transform destinationObject = null;
    [SerializeField] private Vector3 destinationOffset = Vector3.zero;
    [SerializeField] private KeyCode teleportKey = KeyCode.T;
    [SerializeField] private bool resetVelocity = true;

    [Header("Effects")]
    [SerializeField] private bool showTeleportMessage = true;
    [SerializeField] private string teleportMessage = "¡Teletransportado!";

    private Rigidbody2D rb;

    void Start()
    {
        // Cachear el Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Si presiona la tecla, teletransportar
        if (Input.GetKeyDown(teleportKey))
        {
            TeleportPlayer();
        }
    }

    void TeleportPlayer()
    {
        // Parar movimiento si es necesario
        if (resetVelocity && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Determinar posición final
        Vector3 finalPosition;
        if (destinationObject != null)
        {
            // Usar GameObject + offset
            finalPosition = destinationObject.position + destinationOffset;
        }
        else
        {
            // Usar posición manual
            finalPosition = teleportPosition;
        }

        // Teletransportar
        transform.position = finalPosition;

        // Mostrar mensaje
        if (showTeleportMessage)
        {
            string destination = destinationObject != null ? destinationObject.name : "posición fija";
            UnityEngine.Debug.Log($"{teleportMessage} - Destino: {destination}");
        }
    }

    // Método público para cambiar la posición de destino
    public void SetTeleportPosition(Vector3 newPosition)
    {
        teleportPosition = newPosition;
        destinationObject = null; // Limpiar objeto destino
    }

    // Método público para cambiar el objeto destino
    public void SetDestinationObject(Transform newDestination, Vector3 offset = default)
    {
        destinationObject = newDestination;
        destinationOffset = offset;
    }

    // Método público para teletransportar desde código
    public void Teleport()
    {
        TeleportPlayer();
    }

    // Visualización en editor
    void OnDrawGizmosSelected()
    {
        // Dibujar posición actual
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        // Determinar destino final
        Vector3 finalDestination;
        if (destinationObject != null)
        {
            finalDestination = destinationObject.position + destinationOffset;
        }
        else
        {
            finalDestination = teleportPosition;
        }

        // Dibujar línea al destino
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, finalDestination);

        // Dibujar destino
        Gizmos.color = destinationObject != null ? Color.cyan : Color.red;
        Gizmos.DrawWireSphere(finalDestination, 0.7f);

        // Etiqueta en el destino
        Gizmos.color = Color.white;
        Gizmos.DrawRay(finalDestination, Vector3.up * 1.5f);

        // Mostrar offset si hay objeto destino
        if (destinationObject != null && destinationOffset != Vector3.zero)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(destinationObject.position, 0.3f);
            Gizmos.DrawLine(destinationObject.position, finalDestination);
        }
    }
}