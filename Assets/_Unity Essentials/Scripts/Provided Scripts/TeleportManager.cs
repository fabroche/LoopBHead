using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] private Transform player;

    void Start()
    {
        // Buscar jugador automáticamente
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    // Teletransportar a una posición específica
    public void TeleportTo(Vector3 position)
    {
        if (player == null) return;

        // Parar movimiento si tiene Rigidbody2D
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Teletransportar
        player.position = position;

        Debug.Log($"Jugador teletransportado a: {position}");
    }

    // Teletransportar a otro Transform
    public void TeleportTo(Transform destination)
    {
        if (destination != null)
        {
            TeleportTo(destination.position);
        }
    }

    // Teletransportar con offset
    public void TeleportTo(Vector3 position, Vector3 offset)
    {
        TeleportTo(position + offset);
    }

    // Métodos públicos para llamar desde el Inspector o código
    public void TeleportToPoint1() => TeleportTo(new Vector3(10, 0, 0));
    public void TeleportToPoint2() => TeleportTo(new Vector3(-10, 5, 0));
    public void TeleportToSpawn() => TeleportTo(Vector3.zero);
}