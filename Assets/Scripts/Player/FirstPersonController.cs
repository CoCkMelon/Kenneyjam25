using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public float jumpForce = 5.0f;
    public float gravity = 9.81f;
    public float airControl = 0.3f;
    
    [Header("Look Settings")]
    public float mouseSensitivity = 2.0f;
    public float touchSensitivity = 2.0f;
    public float maxLookAngle = 80f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    private CharacterController controller;
    private Camera playerCamera;
    private PlayerInput playerInput;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        playerInput = GetComponent<PlayerInput>();
        
        if (playerCamera == null)
            playerCamera = Camera.main;
        
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found! Please add PlayerInput to the player GameObject.");
        }
        
        // Cursor locking managed by InputModeController
        
        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -controller.height/2 - 0.1f, 0);
            groundCheck = groundCheckObj.transform;
        }
    }
    
    void Update()
    {
        GroundCheck();
        HandleMovement();
        HandleLook();
        HandleJump();
    }
    
    void GroundCheck()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
        else
        {
            isGrounded = controller.isGrounded;
        }
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
    
    void HandleMovement()
    {
        if (playerInput == null) return;
        
        Vector2 moveInput = playerInput.MoveInput;
        bool isRunning = playerInput.RunInput;
        
        // Calculate movement direction
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        // Apply air control when not grounded
        if (!isGrounded)
        {
            currentSpeed *= airControl;
        }
        
        controller.Move(move * currentSpeed * Time.deltaTime);
        
        // Apply gravity
        velocity.y += -gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    
    void HandleLook()
    {
        if (playerInput == null) return;
        
        Vector2 lookInput = playerInput.LookInput;
        
        // Rotate the player body left and right
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);
        
        // Rotate the camera up and down
        xRotation -= lookInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
    
    void HandleJump()
    {
        if (playerInput == null) return;
        
        if (playerInput.JumpInput && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * 2f * gravity);
        }
    }
    
    public void Jump()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * 2f * gravity);
        }
    }
    
    // Properties for accessing player state
    public bool IsGrounded => isGrounded;
    public Vector3 Velocity => velocity;
    public float CurrentSpeed => controller.velocity.magnitude;
    
    // Method to set player position (for respawning, teleporting, etc.)
    public void SetPosition(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;
        velocity = Vector3.zero;
    }
    
    // Method to add force to player (for knockback, etc.)
    public void AddForce(Vector3 force)
    {
        velocity += force;
    }
}
