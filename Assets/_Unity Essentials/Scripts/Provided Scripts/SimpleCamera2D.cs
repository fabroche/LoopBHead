using UnityEngine;

public class SimpleCamera2D : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float smoothSpeed = 0.3f;
    public Vector2 offset = Vector2.zero;

    [Header("Rotation Settings")]
    public KeyCode rotationKey = KeyCode.R;
    public float rotationAmount = 90f;

    private Vector3 velocity;

    void LateUpdate()
    {
        // Seguimiento de cámara
        if (target == null) return;

        Vector3 targetPos = target.position + (Vector3)offset;
        targetPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothSpeed
        );

        // Rotación de cámara
        if (Input.GetKeyDown(rotationKey))
        {
            transform.Rotate(0, 0, rotationAmount);
        }
    }
}