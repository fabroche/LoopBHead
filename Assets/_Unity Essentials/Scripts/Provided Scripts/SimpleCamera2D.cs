using UnityEngine;

public class SimpleCamera2D : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.3f;
    public Vector2 offset = Vector2.zero;

    private Vector3 velocity;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + (Vector3)offset;
        targetPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothSpeed
        );
    }
}