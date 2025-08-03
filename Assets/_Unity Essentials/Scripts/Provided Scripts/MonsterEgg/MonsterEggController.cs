using System;
using System.Diagnostics;
using UnityEngine;

/*
Configuración paso a paso:
1. Setup básico:

Crea un GameObject con Collider2D marcado como "Is Trigger"
Añade el script InteractiveObjectRemover
Arrastra el objeto que quieres remover al campo "Object To Remove"

2. Configuración de efectos:
Para efecto de partículas personalizado:

Crea un prefab con ParticleSystem
Arrástralo al campo "Effect Prefab"

Para efecto de sonido:

Arrastra un AudioClip al campo "Sound Effect"
Marca "Create Sound Effect"

3. Configuración de UI (opcional):

Crea un UI Text que diga "Presiona E"
Arrástralo al campo "Prompt UI"

4. Funcionamiento:

Jugador entra en el collider - Se muestra prompt
Presiona E - Se ejecuta RemoveObjectWithEffect()
Se guarda la posición del objeto antes de destruirlo
Se crean efectos en esa posición
Se destruye el objeto

5. Características incluidas:

Detección automática del jugador
Prompt visual opcional
Efectos de partículas (prefab o simple)
Efectos de sonido
Debug visual en el editor
Métodos públicos para control externo
 */

public class InteractiveObjectRemover : MonoBehaviour
{

    [Header("Object to Remove")]
    [SerializeField] private GameObject objectToRemove; // Arrastra aquí el objeto que quieres remover
    [SerializeField] private GameObject objectToRemove2; // Arrastra aquí el objeto que quieres remover

    [Header("Interaction Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    [Header("Effect Settings")]
    [SerializeField] private GameObject effectPrefab; // Prefab del efecto (opcional)
    [SerializeField] private bool createParticleEffect = true;
    [SerializeField] private bool createSoundEffect = true;
    [SerializeField] private AudioClip soundEffect;

    [Header("Visual Feedback")]
    [SerializeField] private bool showInteractionPrompt = true;
    [SerializeField] private GameObject promptUI; // UI que muestra "Presiona E"

    [Header("Debug")]
    [SerializeField] private bool playerInRange = false;

    private AudioSource audioSource;
    private bool playerHaveWeapon = false;

    void Start()
    {
        // Obtener AudioSource si existe
        audioSource = GetComponent<AudioSource>();

        // Ocultar prompt al inicio
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }

        // Verificar que tenemos objeto para remover
        if (objectToRemove == null)
        {
            UnityEngine.Debug.LogWarning($"{gameObject.name}: No se ha asignado objeto para remover!");
        }
    }

    void Update()
    {
        // Solo verificar input si el jugador está en rango
        if (playerInRange && playerHaveWeapon && Input.GetKeyDown(interactionKey))
        {
            RemoveObjectWithEffect();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            // Verificar si el jugador tiene el arma
            playerHaveWeapon = other.gameObject.GetComponent<ButtHeadController>().haveWeapon;

            playerInRange = true;

            // Mostrar prompt de interacción
            if (promptUI != null)
            {
                promptUI.SetActive(true);
            }

            UnityEngine.Debug.Log($"Jugador en rango. Presiona {interactionKey} para interactuar");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;

            // Ocultar prompt de interacción
            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }

            UnityEngine.Debug.Log("Jugador salió del rango");
        }
    }

    void RemoveObjectWithEffect()
    {
        if (objectToRemove == null && objectToRemove2 == null)
        {
            UnityEngine.Debug.LogWarning("No hay objeto para remover!");
            return;
        }

        // Guardar posición antes de remover
        Vector3 effectPosition = objectToRemove.transform.position;
        Vector3 effectPosition2 = objectToRemove2.transform.position;

        UnityEngine.Debug.Log($"Removiendo objeto: {objectToRemove.name}");
        UnityEngine.Debug.Log($"Removiendo objeto: {objectToRemove2.name}");

        // Crear efectos en la posición del objeto
        CreateEffects(effectPosition);
        CreateEffects(effectPosition2);

        // Remover el objeto
        Destroy(objectToRemove);
        Destroy(objectToRemove2);

        // Opcional: También remover este collider después de usar
        Destroy(gameObject, 0.5f);
    }

    void CreateEffects(Vector3 position)
    {
        // 1. Efecto de partículas personalizado
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);

            // Auto-destruir el efecto después de un tiempo
            Destroy(effect, 3f);

            UnityEngine.Debug.Log("Efecto de partículas creado");
        }

        // 2. Efecto de partículas simple (si no tienes prefab)
        if (createParticleEffect && effectPrefab == null)
        {
            CreateSimpleParticleEffect(position);
        }

        // 3. Efecto de sonido
        if (createSoundEffect && soundEffect != null)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(soundEffect);
            }
            else
            {
                // Crear AudioSource temporal
                GameObject tempAudio = new GameObject("TempAudio");
                tempAudio.transform.position = position;
                AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
                tempSource.clip = soundEffect;
                tempSource.Play();

                // Destruir después de que termine el sonido
                Destroy(tempAudio, soundEffect.length);
            }

            UnityEngine.Debug.Log("Efecto de sonido reproducido");
        }
    }

    void CreateSimpleParticleEffect(Vector3 position)
    {
        // Crear un efecto simple con GameObjects
        for (int i = 0; i < 8; i++)
        {
            GameObject particle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            particle.transform.position = position;
            particle.transform.localScale = Vector3.one * 0.1f;

            // Añadir color
            Renderer renderer = particle.GetComponent<Renderer>();
            renderer.material.color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

            // Añadir movimiento
            Rigidbody2D rb = particle.AddComponent<Rigidbody2D>();
            Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            rb.AddForce(randomDirection * UnityEngine.Random.Range(100f, 300f));

            // Auto-destruir
            Destroy(particle, UnityEngine.Random.Range(1f, 2f));
        }

        UnityEngine.Debug.Log("Efecto de partículas simple creado");
    }

    // Método público para llamar desde otros scripts
    public void ForceRemoveObject()
    {
        RemoveObjectWithEffect();
    }

    // Método para cambiar el objeto a remover desde código
    public void SetObjectToRemove(GameObject newObject)
    {
        objectToRemove = newObject;
    }

    // Visualización en editor
    void OnDrawGizmosSelected()
    {
        // Mostrar área de interacción
        Gizmos.color = Color.green;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }

        // Mostrar línea hacia el objeto a remover
        if (objectToRemove != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, objectToRemove.transform.position);
            Gizmos.DrawWireSphere(objectToRemove.transform.position, 0.5f);
        }
    }
}