using UnityEngine;

public class SmoothFollowCamera2D5 : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Smoothing")]
    [Range(0.1f, 1f)] public float positionSmoothTime = 0.15f;
    [Range(0.1f, 1f)] public float rotationSmoothTime = 0.1f;

    [Header("Offset")]
    public Vector3 positionOffset = new Vector3(0, 0, -10);
    public Vector3 lookOffset = Vector3.zero;

    private Vector3 currentVelocity;
    private Vector3 currentRotationVelocity;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("Camera component not found on this GameObject!");
            enabled = false;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + positionOffset;

        // Apply smoothing to position
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            positionSmoothTime
        );

        // Calculate desired rotation
        Quaternion desiredRotation = Quaternion.LookRotation(
            target.position + lookOffset - transform.position,
            Vector3.up
        );

        // Apply smoothing to rotation
        Quaternion smoothedRotation = Quaternion.Slerp(
            transform.rotation,
            desiredRotation,
            rotationSmoothTime
        );

        // Update camera position and rotation
        transform.position = smoothedPosition;
        transform.rotation = smoothedRotation;
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;

        // Draw camera offset indicator
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.matrix = Matrix4x4.TRS(target.position, target.rotation, Vector3.one);
        Gizmos.DrawWireCube(positionOffset, Vector3.one * 0.5f);
    }
}
