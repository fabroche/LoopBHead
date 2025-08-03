/*
Cómo configurarlo:
1. Setup del Trigger:

Crea un GameObject vacío (será tu zona de activación)
Añade el script ChasingTrigger
Añade un Collider2D y marca "Is Trigger"
Arrastra tu monstruo al campo "Monster Object"

2. Setup de los puntos de patrullaje:

Crea 2 GameObjects vacíos como puntos de patrullaje
Posiciónalos donde quieres que patrulla el monstruo
Arrastra estos puntos a "Patrol Point 1" y "Patrol Point 2"

Configuración:

Monster Object: Tu monstruo/enemigo
Patrol Point 1: Primer punto de patrullaje
Patrol Point 2: Segundo punto de patrullaje
Patrol Speed: Velocidad de patrullaje (2 por defecto)
Destroy Trigger After Activation: Si destruir el trigger tras activarlo

Cómo funciona:

Jugador entra en el trigger
Se añade automáticamente el componente SimplePatrol al monstruo
El monstruo empieza a moverse entre los dos puntos
El trigger se destruye (opcional)

Características:
Súper simple: Solo arrastra referencias y funciona
Auto-setup: Añade el componente automáticamente
Inteligente: Va al punto más cercano primero
Visual: Puedes ver las rutas en el editor
Flexible: Puedes cambiar velocidad y puntos
 */
using System.Diagnostics;
using UnityEngine;

public class ChasingTrigger : MonoBehaviour
{
    [Header("Monster Reference")]
    public GameObject monsterObject;

    [Header("Patrol Settings")]
    public Transform patrolPoint1;
    public Transform patrolPoint2;
    public float patrolSpeed = 2f;

    [Header("Trigger Settings")]
    public bool destroyTriggerAfterActivation = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ActivatePatrol();

            if (destroyTriggerAfterActivation)
            {
                Destroy(gameObject);
            }
        }
    }

    void ActivatePatrol()
    {
        if (monsterObject == null) return;

        // Añadir el componente de patrullaje al monstruo
        SimplePatrol patrol = monsterObject.GetComponent<SimplePatrol>();

        if (patrol == null)
        {
            patrol = monsterObject.AddComponent<SimplePatrol>();
        }

        // Configurar el patrullaje
        patrol.SetPatrolPoints(patrolPoint1, patrolPoint2);
        patrol.SetPatrolSpeed(patrolSpeed);
        patrol.StartPatrolling();

        UnityEngine.Debug.Log($"{monsterObject.name} comenzó a patrullar!");
    }
}

// Script de patrullaje simple para el monstruo
public class SimplePatrol : MonoBehaviour
{
    private Transform point1;
    private Transform point2;
    private Transform currentTarget;
    private float speed = 2f;
    private bool isPatrolling = false;
    private float velocityXthreshold = 0.1f;

    private Animator monsterAnimator;
    private Rigidbody2D rb;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        monsterAnimator = GetComponent<Animator>();
        if (monsterAnimator == null)
        {
            UnityEngine.Debug.LogWarning("No se encontró Animator en el monstruo. Las animaciones de patrullaje no funcionarán.");
        }
    }

    void Update()
    {
        if (!isPatrolling || currentTarget == null) return;

        monsterAnimator.SetBool("isPatroling", isPatrolling);

        FlipAnimation();

        // Mover hacia el punto objetivo
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget.position,
            speed * Time.deltaTime
        );

        // Si llegó al punto, cambiar al otro
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            SwitchTarget();
        }
    }

    void FixedUpdate()
    {
        if (isPatrolling && rb != null)
        {
            // Actualizar la dirección de patrullaje
            // patrolingDirection = GetPatrolingDirection();


        }
    }

    void FlipAnimation()
    {
        // UnityEngine.Debug.Log($"Patrullando en dirección: {currentTarget}");

        if (currentTarget == point1)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);

        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void SetPatrolPoints(Transform p1, Transform p2)
    {
        point1 = p1;
        point2 = p2;

        // Empezar hacia el punto más cercano
        if (point1 != null && point2 != null)
        {
            float dist1 = Vector3.Distance(transform.position, point1.position);
            float dist2 = Vector3.Distance(transform.position, point2.position);

            currentTarget = (dist1 < dist2) ? point1 : point2;
        }
    }

    public void SetPatrolSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void StartPatrolling()
    {
        if (point1 != null && point2 != null)
        {
            isPatrolling = true;
        }
        else
        {
            UnityEngine.Debug.LogWarning("No se pueden establecer puntos de patrullaje. Asigna point1 y point2.");
        }
    }

    public void StopPatrolling()
    {
        isPatrolling = false;
    }

    void SwitchTarget()
    {
        currentTarget = (currentTarget == point1) ? point2 : point1;
    }

    private int GetPatrolingDirection()
    {
        if (rb == null) return 0;

        float velX = rb.linearVelocity.x;

        if (velX > velocityXthreshold)
        {
        return 1;  // Derecha
        }
        else if (velX < -velocityXthreshold)
        {

            return -1; // Izquierda
        }
        else
            return 0;  // No se está patrullando
    }

    // Visualización en editor
    void OnDrawGizmosSelected()
    {
        if (point1 == null || point2 == null) return;

        // Dibujar línea entre puntos
        Gizmos.color = Color.red;
        Gizmos.DrawLine(point1.position, point2.position);

        // Dibujar puntos
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(point1.position, 0.3f);
        Gizmos.DrawWireSphere(point2.position, 0.3f);

        // Dibujar objetivo actual
        if (currentTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
}