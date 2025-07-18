using UnityEngine;

/// <summary>
/// Base PlayerController class that the HUD system expects.
/// This acts as a wrapper/interface for the FirstPersonController.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private FirstPersonController firstPersonController;
    
    void Awake()
    {
        firstPersonController = GetComponent<FirstPersonController>();
        if (firstPersonController == null)
        {
            Debug.LogError("FirstPersonController component not found on PlayerController!");
        }
    }
    
    // Properties that other systems can access
    public bool IsGrounded => firstPersonController != null ? firstPersonController.IsGrounded : false;
    public Vector3 Velocity => firstPersonController != null ? firstPersonController.Velocity : Vector3.zero;
    public float CurrentSpeed => firstPersonController != null ? firstPersonController.CurrentSpeed : 0f;
    
    // Methods that other systems can call
    public void Jump()
    {
        if (firstPersonController != null)
            firstPersonController.Jump();
    }
    
    public void SetPosition(Vector3 position)
    {
        if (firstPersonController != null)
            firstPersonController.SetPosition(position);
    }
    
    public void AddForce(Vector3 force)
    {
        if (firstPersonController != null)
            firstPersonController.AddForce(force);
    }
}
