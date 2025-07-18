using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Tooltip("The player's Transform to follow.")]
    public Transform player;  // Assign this in the Inspector

    [Tooltip("Smoothing speed for camera movement. Lower values = smoother (e.g., 0.1f), higher values = faster (e.g., 1f).")]
    [Range(0.01f, 1f)]  // Limits the range for easier tweaking
    public float smoothSpeed = 0.125f;  // Default smoothing factor

    private Vector3 velocity = Vector3.zero;  // Velocity vector for SmoothDamp

    void LateUpdate()
    {
        if (player != null)
        {
            // Calculate the desired position based on the player's position
            Vector3 desiredPosition = player.position;

            // For 2.5D, lock the Z-axis to the camera's current Z (e.g., to maintain depth)
            desiredPosition.z = transform.position.z;

            // Smoothly interpolate the camera's position towards the desired position
            Vector3 smoothedPosition = Vector3.SmoothDamp(
                transform.position,  // Current camera position
                desiredPosition,     // Target position
                ref velocity,        // Reference to velocity for smoothing
                smoothSpeed          // Smoothing factor (time to reach target)
            );

            // Apply the smoothed position to the camera
            transform.position = smoothedPosition;
        }
        else
        {
            Debug.LogWarning("Player Transform is not assigned in CameraFollow2D script.");
        }
    }
}
