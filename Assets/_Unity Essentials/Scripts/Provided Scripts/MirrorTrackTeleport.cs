using System.Diagnostics;
using UnityEngine;

public class MirrorTrackTeleport : MonoBehaviour
{
    [Header("Track References")]
    [SerializeField] private Transform trackA_Start; // Inicio de pista A
    [SerializeField] private Transform trackA_End;   // Final de pista A
    [SerializeField] private Transform trackB_Start; // Inicio de pista B
    [SerializeField] private Transform trackB_End;   // Final de pista B

    [Header("Settings")]
    [SerializeField] private KeyCode teleportKey = KeyCode.T;
    [SerializeField] private bool resetVelocity = true;
    [SerializeField] private bool showDebugInfo = true;

    [Header("Current Track")]
    [SerializeField] private TrackType currentTrack = TrackType.TrackA;

    public enum TrackType
    {
        TrackA,
        TrackB
    }

    private Rigidbody2D rb;
    private ButtHeadController _playerScript;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _playerScript = GetComponent<ButtHeadController>();
        
        // Verificar que todas las referencias est�n asignadas
        if (trackA_Start == null || trackA_End == null || trackB_Start == null || trackB_End == null)
        {
            UnityEngine.Debug.LogError("�Faltan referencias de las pistas! Asigna todos los puntos de inicio y final.");
        }
    }

    void Update()
    {
        if (!_playerScript.isCarryingMonsterEgg &&Input.GetKeyDown(teleportKey))
        {
            TeleportToMirrorTrack();
        }

        // Auto-detectar en qu� pista est� el jugador
        if (showDebugInfo)
        {
            DetectCurrentTrack();
        }
    }

    void TeleportToMirrorTrack()
    {
        if (!AreReferencesValid()) return;

        Vector3 currentPosition = transform.position;
        Vector3 mirrorPosition;

        if (currentTrack == TrackType.TrackA)
        {
            // Calcular posici�n equivalente en pista B
            mirrorPosition = CalculateMirrorPosition(
                currentPosition,
                trackA_Start.position,
                trackA_End.position,
                trackB_Start.position,
                trackB_End.position
            );
            currentTrack = TrackType.TrackB;
        }
        else
        {
            // Calcular posici�n equivalente en pista A
            mirrorPosition = CalculateMirrorPosition(
                currentPosition,
                trackB_Start.position,
                trackB_End.position,
                trackA_Start.position,
                trackA_End.position
            );
            currentTrack = TrackType.TrackA;
        }

        // Ejecutar teletransporte
        PerformTeleport(mirrorPosition);

        if (showDebugInfo)
        {
            UnityEngine.Debug.Log($"Teletransportado de {(currentTrack == TrackType.TrackB ? "A" : "B")} a {currentTrack} - Posici�n: {mirrorPosition}");
        }
    }

    Vector3 CalculateMirrorPosition(Vector3 currentPos, Vector3 fromStart, Vector3 fromEnd, Vector3 toStart, Vector3 toEnd)
    {
        // Calcular el porcentaje de progreso en la pista actual
        Vector3 trackVector = fromEnd - fromStart;
        Vector3 playerVector = currentPos - fromStart;

        // Proyectar la posici�n del jugador sobre la l�nea de la pista
        float trackLength = trackVector.magnitude;
        if (trackLength == 0) return toStart; // Evitar divisi�n por cero

        Vector3 trackDirection = trackVector.normalized;
        float projectedDistance = Vector3.Dot(playerVector, trackDirection);

        // Calcular porcentaje (0 = inicio, 1 = final)
        float percentage = Mathf.Clamp01(projectedDistance / trackLength);

        // Aplicar el mismo porcentaje en la pista destino
        Vector3 targetTrackVector = toEnd - toStart;
        Vector3 mirrorPosition = toStart + (targetTrackVector * percentage);

        // Mantener la altura Y relativa si las pistas tienen diferentes alturas
        float heightOffset = currentPos.y - (fromStart.y + (fromEnd.y - fromStart.y) * percentage);
        mirrorPosition.y += heightOffset;

        return mirrorPosition;
    }

    void PerformTeleport(Vector3 destination)
    {
        // Parar movimiento
        if (resetVelocity && rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Teletransportar
        transform.position = destination;
    }

    void DetectCurrentTrack()
    {
        if (!AreReferencesValid()) return;

        Vector3 currentPos = transform.position;

        // Calcular distancia a cada pista
        float distanceToTrackA = DistanceToLine(currentPos, trackA_Start.position, trackA_End.position);
        float distanceToTrackB = DistanceToLine(currentPos, trackB_Start.position, trackB_End.position);

        // Determinar pista m�s cercana
        currentTrack = (distanceToTrackA < distanceToTrackB) ? TrackType.TrackA : TrackType.TrackB;
    }

    float DistanceToLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineVector = lineEnd - lineStart;
        Vector3 pointVector = point - lineStart;

        float lineLength = lineVector.magnitude;
        if (lineLength == 0) return Vector3.Distance(point, lineStart);

        Vector3 lineDirection = lineVector / lineLength;
        float projectedLength = Vector3.Dot(pointVector, lineDirection);
        projectedLength = Mathf.Clamp(projectedLength, 0, lineLength);

        Vector3 closestPoint = lineStart + lineDirection * projectedLength;
        return Vector3.Distance(point, closestPoint);
    }

    bool AreReferencesValid()
    {
        return trackA_Start != null && trackA_End != null && trackB_Start != null && trackB_End != null;
    }

    // M�todos p�blicos para control externo
    public void SetCurrentTrack(TrackType track)
    {
        currentTrack = track;
    }

    public void TeleportToSpecificTrack(TrackType targetTrack)
    {
        if (currentTrack != targetTrack)
        {
            TeleportToMirrorTrack();
        }
    }

    // Configurar pistas desde c�digo
    public void SetupTracks(Transform aStart, Transform aEnd, Transform bStart, Transform bEnd)
    {
        trackA_Start = aStart;
        trackA_End = aEnd;
        trackB_Start = bStart;
        trackB_End = bEnd;
    }

    // Visualizaci�n en editor
    void OnDrawGizmosSelected()
    {
        if (!AreReferencesValid()) return;

        // Dibujar pista A
        Gizmos.color = Color.red;
        Gizmos.DrawLine(trackA_Start.position, trackA_End.position);
        Gizmos.DrawWireSphere(trackA_Start.position, 0.3f);
        Gizmos.DrawWireSphere(trackA_End.position, 0.3f);

        // Dibujar pista B
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(trackB_Start.position, trackB_End.position);
        Gizmos.DrawWireSphere(trackB_Start.position, 0.3f);
        Gizmos.DrawWireSphere(trackB_End.position, 0.3f);

        // Dibujar jugador y su posici�n espejo
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        // Calcular y mostrar posici�n espejo
        Vector3 mirrorPos;
        if (currentTrack == TrackType.TrackA)
        {
            mirrorPos = CalculateMirrorPosition(
                transform.position,
                trackA_Start.position,
                trackA_End.position,
                trackB_Start.position,
                trackB_End.position
            );
        }
        else
        {
            mirrorPos = CalculateMirrorPosition(
                transform.position,
                trackB_Start.position,
                trackB_End.position,
                trackA_Start.position,
                trackA_End.position
            );
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(mirrorPos, 0.4f);
        Gizmos.DrawLine(transform.position, mirrorPos);
    }
}