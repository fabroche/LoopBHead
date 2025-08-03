using UnityEngine;

public class SimpleCameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Settings")]
    [SerializeField] private Vector2 offset = Vector2.zero;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private float jumpIgnoreThreshold = 1f; // Distancia mínima para ignorar movimientos Y

    // Variables privadas
    private Vector2 velocity = Vector2.zero;
    private float lastGroundY;
    private Rigidbody2D targetRb;

    void Start()
    {
        // Buscar target automáticamente si no está asignado
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        // Cachear componentes
        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
            lastGroundY = target.position.y;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        FollowTarget();
    }

    void FollowTarget()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = target.position + (Vector3)offset;

        // Siempre seguir en X
        targetPos.x = target.position.x + offset.x;

        // Solo seguir en Y si el jugador está en el suelo o se alejó mucho
        if (IsPlayerGrounded())
        {
            // Actualizar la Y del suelo cuando el jugador toca tierra
            lastGroundY = target.position.y;
            targetPos.y = lastGroundY + offset.y;
        }
        else
        {
            // Si está saltando, usar la última Y del suelo
            targetPos.y = lastGroundY + offset.y;

            // Solo cambiar si se alejó demasiado (para plataformas altas)
            float yDistance = Mathf.Abs(target.position.y - lastGroundY);
            if (yDistance > jumpIgnoreThreshold)
            {
                targetPos.y = target.position.y + offset.y;
                lastGroundY = target.position.y; // Actualizar nueva altura base
            }
        }

        // Mantener Z de la cámara
        targetPos.z = currentPos.z;

        // Movimiento suave
        transform.position = Vector2.SmoothDamp(
            currentPos,
            targetPos,
            ref velocity,
            smoothTime
        );
    }

    bool IsPlayerGrounded()
    {
        if (targetRb == null) return false;

        // Simple: si la velocidad Y es casi cero, está en el suelo
        return Mathf.Abs(targetRb.linearVelocity.y) < 0.1f;
    }

    // Método público para cambiar el objetivo
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
            lastGroundY = target.position.y;
        }
    }
}