using System;
using System.Diagnostics;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

/*
Configuraci�n paso a paso:
1. Setup b�sico:

Crea un GameObject con Collider2D marcado como "Is Trigger"
A�ade el script InteractiveObjectRemover
Arrastra el objeto que quieres remover al campo "Object To Remove"

2. Configuraci�n de efectos:
Para efecto de part�culas personalizado:

Crea un prefab con ParticleSystem
Arr�stralo al campo "Effect Prefab"

Para efecto de sonido:

Arrastra un AudioClip al campo "Sound Effect"
Marca "Create Sound Effect"

3. Configuraci�n de UI (opcional):

Crea un UI Text que diga "Presiona E"
Arr�stralo al campo "Prompt UI"

4. Funcionamiento:

Jugador entra en el collider - Se muestra prompt
Presiona E - Se ejecuta RemoveObjectWithEffect()
Se guarda la posici�n del objeto antes de destruirlo
Se crean efectos en esa posici�n
Se destruye el objeto

5. Caracter�sticas incluidas:

Detecci�n autom�tica del jugador
Prompt visual opcional
Efectos de part�culas (prefab o simple)
Efectos de sonido
Debug visual en el editor
M�todos p�blicos para control externo
 */

public class InteractiveObjectRemover : MonoBehaviour
{
    [Header("Object to Remove")] [SerializeField, Tooltip("Drag the object you want to remove in the Future here")]
    private GameObject objectToRemoveFuture; // Future Object (Monster)

    [SerializeField, Tooltip("Drag the object you want to remove in the Past here")]
    private GameObject objectToRemovePast; // Past Object (Egg)

    [Header("Interaction Settings")] private KeyCode attackKey = KeyCode.J;
    private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private string playerTag = "Player";

    [Header("Effect Settings")] [SerializeField]
    private GameObject effectPrefab; // Prefab del efecto (opcional)

    [SerializeField] private bool createParticleEffect = true;
    [SerializeField] private bool createSoundEffect = true;
    [SerializeField] private AudioClip soundEffect;

    [Header("Visual Feedback")] [SerializeField]
    private bool showInteractionPrompt = true;

    [SerializeField] private GameObject promptUI; // UI que muestra "Presiona E"

    [Header("Debug")] [SerializeField] private bool playerInRange = false;
    
    [Header("Main Character Position")] [SerializeField]
    private Transform mainCharacterPosition;

    private AudioSource audioSource;
    private bool playerHaveWeapon = false;
    private bool playerIsCarryingTheEgg = false;
    

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
        if (objectToRemoveFuture == null)
        {
            UnityEngine.Debug.LogWarning($"{gameObject.name}: No se ha asignado objeto para remover!");
        }
    }

    void Update()
    {
        
        if (playerIsCarryingTheEgg)
        {
            objectToRemovePast.transform.position = mainCharacterPosition.position + Vector3.up * 1.2f;
        }
        
        // Solo verificar input si el jugador est� en rango
        if (playerInRange && playerHaveWeapon && Input.GetKeyDown(attackKey))
        {
            RemoveObjectWithEffect();
        }

        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            if (playerIsCarryingTheEgg)
            {
                DropEgg();
            }
            else
            {
                PickUpEgg();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ButtHeadController playerController = other.gameObject.GetComponent<ButtHeadController>();

        attackKey = playerController.attackKey;
        interactionKey = playerController.insteractionKey;

        if (other.CompareTag(playerTag))
        {
            // Verificar si el jugador tiene el arma
            playerHaveWeapon = playerController.haveWeapon;

            playerInRange = true;

            // Mostrar prompt de interacci�n
            if (promptUI != null)
            {
                promptUI.SetActive(true);
            }

            UnityEngine.Debug.Log($"Jugador en rango. Presiona {attackKey} para interactuar");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;

            // Ocultar prompt de interacci�n
            if (promptUI != null)
            {
                promptUI.SetActive(false);
            }

            UnityEngine.Debug.Log("Jugador sali� del rango");
        }
    }

    void RemoveObjectWithEffect()
    {
        if (objectToRemoveFuture == null && objectToRemovePast == null)
        {
            UnityEngine.Debug.LogWarning("No hay objeto para remover!");
            return;
        }

        // Guardar posici�n antes de remover
        Vector3 effectPosition = objectToRemoveFuture.transform.position;
        Vector3 effectPosition2 = objectToRemovePast.transform.position;

        UnityEngine.Debug.Log($"Removiendo objeto: {objectToRemoveFuture.name}");
        UnityEngine.Debug.Log($"Removiendo objeto: {objectToRemovePast.name}");

        // Crear efectos en la posici�n del objeto
        CreateEffects(effectPosition);
        CreateEffects(effectPosition2);

        // Remover el objeto
        Destroy(objectToRemoveFuture);
        Destroy(objectToRemovePast);

        // Opcional: Tambi�n remover este collider despu�s de usar
        Destroy(gameObject, 0.5f);
    }

    void PickUpEgg()
    {
        playerIsCarryingTheEgg = true;
        // objectToRemovePast.transform.localPosition = new Vector3(objectToRemovePast.transform.localPosition.x, objectToRemovePast.transform.position.y + 5f, 0);
    }

    void DropEgg()
    {
        playerIsCarryingTheEgg = false;
        Vector3 dropPosition = mainCharacterPosition.rotation.y >= 0 ? Vector3.right : Vector3.left;
        objectToRemovePast.transform.position = mainCharacterPosition.position + Vector3.down * 0.2f + dropPosition * 1.1f;
        
    }
    void CreateEffects(Vector3 position)
    {
        // 1. Efecto de part�culas personalizado
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);

            // Auto-destruir el efecto despu�s de un tiempo
            Destroy(effect, 3f);

            UnityEngine.Debug.Log("Efecto de part�culas creado");
        }

        // 2. Efecto de part�culas simple (si no tienes prefab)
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

                // Destruir despu�s de que termine el sonido
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

            // A�adir color
            Renderer renderer = particle.GetComponent<Renderer>();
            renderer.material.color =
                new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

            // A�adir movimiento
            Rigidbody2D rb = particle.AddComponent<Rigidbody2D>();
            Vector2 randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f))
                .normalized;
            rb.AddForce(randomDirection * UnityEngine.Random.Range(100f, 300f));

            // Auto-destruir
            Destroy(particle, UnityEngine.Random.Range(1f, 2f));
        }

        UnityEngine.Debug.Log("Efecto de part�culas simple creado");
    }

    // M�todo p�blico para llamar desde otros scripts
    public void ForceRemoveObject()
    {
        RemoveObjectWithEffect();
    }

    // M�todo para cambiar el objeto a remover desde c�digo
    public void SetObjectToRemove(GameObject newObject)
    {
        objectToRemoveFuture = newObject;
    }

    // Visualizaci�n en editor
    void OnDrawGizmosSelected()
    {
        // Mostrar �rea de interacci�n
        Gizmos.color = Color.green;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }

        // Mostrar l�nea hacia el objeto a remover
        if (objectToRemoveFuture != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, objectToRemoveFuture.transform.position);
            Gizmos.DrawWireSphere(objectToRemoveFuture.transform.position, 0.5f);
        }
    }
}