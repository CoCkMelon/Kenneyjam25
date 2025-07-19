using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Main controller for the self-moving sleight that collects power to become faster and stronger
/// </summary>
public class SleightController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float maxSpeed = 1000f; // Speed of light threshold
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float turnSpeed = 180f;
    [SerializeField] private float turnSmoothing = 5f;
    [SerializeField] private float currentSpeed = 0f;

    [Header("Force Settings")]
    [SerializeField] private float engineForce = 1500f; // Newtons when throttle is full
    [SerializeField] private float airControlMultiplier = 0.2f;
    [Tooltip("0-1; how much of sideways velocity is redirected forward each physics step when grounded")]
    [SerializeField] private float sideConversionFactor = 0.5f;
    
    [Header("Physics Tuning")]
    [Tooltip("Higher value = stronger resistance to unwanted yaw while grounded")]
    [SerializeField] private float rotationFriction = 5f;
    
    [Header("Power System")]
    [SerializeField] private float powerLevel = 0f;
    [SerializeField] private float maxPowerLevel = 1000f;
    [SerializeField] private float powerToSpeedMultiplier = 10f;
    [SerializeField] private float powerToStrengthMultiplier = 5f;
    
    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask = -1;
    
    [Header("Auto-Movement")]
    [SerializeField] private bool autoMove = true;
    [SerializeField] private float autoMoveInputDelay = 0.5f;
    [SerializeField] private LayerMask obstacleLayer = -1;
    [SerializeField] private float obstacleDetectionDistance = 3f;
    
    [Header("Effects")]
    [SerializeField] private ParticleSystem movementTrail;
    [SerializeField] private ParticleSystem powerCollectionEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip powerCollectSound;
    [SerializeField] private AudioClip speedUpSound;
    
    [Header("Events")]
    public UnityEvent<float> OnPowerChanged;
    public UnityEvent<float> OnSpeedChanged;
    public UnityEvent OnLightSpeedReached;
    
    // Components
    private Rigidbody rb;
    private PlayerInput playerInput;
    private SleightPowerSystem powerSystem;
    private CutsceneManager cutsceneManager;
    
    // Movement state
    private Vector3 movementDirection = Vector3.forward;
    private float throttleInput = 0f;
    private float steeringInput = 0f;
    private float currentYawRate = 0f;
    // Obstacle avoidance state
    private int avoidanceSign = 0; // -1 = left, 1 = right, 0 = none
    private bool hasPlayerInput = false;
    private bool isLightSpeedReached = false;
    private float lastInputTime = 0f;
    private bool isGrounded = false;
    
    // Properties for other systems
    public float CurrentSpeed => currentSpeed;
    public float PowerLevel => powerLevel;
    public float MaxPowerLevel => maxPowerLevel;
    public float SpeedPercentage => Mathf.Abs(currentSpeed) / maxSpeed;
    public bool IsAutoMoving => autoMove && (Time.time - lastInputTime > autoMoveInputDelay);
    public bool IsLightSpeedReached => isLightSpeedReached;
    public bool IsGrounded => isGrounded;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        powerSystem = GetComponent<SleightPowerSystem>();
        cutsceneManager = FindFirstObjectByType<CutsceneManager>();
        
        if (rb == null)
            Debug.LogError("SleightController requires a Rigidbody component!");
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        // Create ground check if it doesn't exist
        if (groundCheck == null)
        {
            GameObject groundCheckObj = new GameObject("GroundCheck");
            groundCheckObj.transform.SetParent(transform);
            groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = groundCheckObj.transform;
        }
    }
    
    void Start()
    {
        // Initialize starting speed
        currentSpeed = baseSpeed;
        
        // Setup power system callbacks
        if (powerSystem != null)
        {
            powerSystem.OnPowerCollected.AddListener(HandlePowerCollected);
            powerSystem.OnPowerLevelChanged.AddListener(HandlePowerLevelChanged);
        }
        
        // Initialize particle systems
        if (movementTrail != null)
            movementTrail.Play();
    }
    
    void Update()
    {
        CheckGrounded();
        HandleInput();
        UpdateMovement();
        UpdateEffects();
        CheckLightSpeedThreshold();
    }
    
    void FixedUpdate()
    {
        ApplyMovement();
    }
    
    private void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
        else
        {
            // Fallback: check if rigidbody is close to ground using a raycast
            isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.0f, groundMask);
        }
    }
    
    private void HandleInput()
    {
        if (playerInput == null || isLightSpeedReached) return;
        
        Vector2 moveInput = playerInput.MoveInput;
        throttleInput = moveInput.y; // -1 back, +1 forward
        steeringInput = moveInput.x; // -1 left, +1 right
        
        hasPlayerInput = moveInput.magnitude > 0.1f;
        if (hasPlayerInput)
        {
            lastInputTime = Time.time;
        }
        else if (IsAutoMoving)
        {
            // auto-move forward when no input
            throttleInput = 1f;
        }
        
        // Smooth steering: interpolate current yaw rate towards desired rate
        float desiredYawRate = steeringInput * turnSpeed;
        currentYawRate = Mathf.Lerp(currentYawRate, desiredYawRate, turnSmoothing * Time.deltaTime);
        transform.Rotate(0f, currentYawRate * Time.deltaTime, 0f);
        
        // Movement direction is always current forward (sign decides fwd/back)
        movementDirection = transform.forward * Mathf.Sign(throttleInput);
    }
    
    private void UpdateMovement()
    {
        if (isLightSpeedReached) return;
        
        // Auto-movement logic (only when no manual input)
        if (IsAutoMoving && !hasPlayerInput)
        {
            // Simple obstacle avoidance
            Vector3 avoidanceDirection = GetObstacleAvoidanceDirection();
            if (avoidanceDirection != Vector3.zero)
            {
                movementDirection = Vector3.Slerp(movementDirection, avoidanceDirection, Time.deltaTime * 2f);
            }
        }
        
        // Translation forces handled in FixedUpdate
    }
    
    private Vector3 GetObstacleAvoidanceDirection()
    {
        // Cast rays to detect obstacles
        Vector3 forward = movementDirection;
        Vector3 left = Vector3.Cross(forward, Vector3.up).normalized;
        Vector3 right = -left;

        bool obstacleAhead = Physics.Raycast(transform.position, forward, obstacleDetectionDistance, obstacleLayer);
        
        if (!obstacleAhead)
        {
            // Path clear, reset avoidance
            avoidanceSign = 0;
            return Vector3.zero;
        }

        // If already avoiding, keep same direction if still blocked
        if (avoidanceSign != 0)
        {
            return avoidanceSign == -1 ? left : right;
        }

        // Decide which side to take
        bool leftClear = !Physics.Raycast(transform.position, left, obstacleDetectionDistance, obstacleLayer);
        bool rightClear = !Physics.Raycast(transform.position, right, obstacleDetectionDistance, obstacleLayer);

        if (leftClear && rightClear)
        {
            avoidanceSign = Random.value > 0.5f ? -1 : 1;
        }
        else if (leftClear)
        {
            avoidanceSign = -1;
        }
        else if (rightClear)
        {
            avoidanceSign = 1;
        }
        else
        {
            // Neither side clear, turn around
            avoidanceSign = 1;
            return -forward;
        }

        return avoidanceSign == -1 ? left : right;
    }
    
    private void ApplyMovement()
    {
        if (rb == null || isLightSpeedReached) return;
        
        // Apply rotational friction only when grounded to resist unwanted yaw but still allow physical forces
        if (isGrounded)
        {
            Vector3 av = rb.angularVelocity;
            // Zero roll and pitch to keep sled upright
            av.x = 0f;
            av.z = 0f;
            // Gradually damp yaw (Y) velocity toward zero like friction
            av.y = Mathf.Lerp(av.y, 0f, rotationFriction * Time.fixedDeltaTime);
            rb.angularVelocity = av;
        }
        
        // Compute throttle to use this frame
        float throttle = throttleInput;
        if (!hasPlayerInput && IsAutoMoving)
        {
            throttle = 1f; // full ahead
        }

        // Scale force based on grounded / air
        float forceMul = isGrounded ? 1f : airControlMultiplier;
        rb.AddForce(transform.forward * throttle * engineForce * forceMul, ForceMode.Force);

        // Convert some sideways velocity into forward when grounded (sled carving)
        if (isGrounded && sideConversionFactor > 0f)
        {
            Vector3 vel = rb.linearVelocity;
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            float sideSpeed = Vector3.Dot(vel, right);
            Vector3 sideComponent = right * sideSpeed;
            // amount to convert this frame
            Vector3 convert = sideComponent * sideConversionFactor;
            vel -= convert;               // remove from sideways
            vel += forward * Mathf.Abs(sideSpeed) * sideConversionFactor; // add forward
            rb.linearVelocity = vel;
        }

        // Clamp max speed based on power level
        float speedCap = Mathf.Clamp(baseSpeed + powerLevel * powerToSpeedMultiplier, baseSpeed, maxSpeed);
        if (rb.linearVelocity.magnitude > speedCap)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speedCap;
        }

        // Update currentSpeed for UI/events
        float prevSpeed = currentSpeed;
        currentSpeed = rb.linearVelocity.magnitude;
        if (!Mathf.Approximately(prevSpeed, currentSpeed))
        {
            OnSpeedChanged?.Invoke(currentSpeed);
        }
    }
    
    private void UpdateEffects()
    {
        // Update movement trail based on speed
        if (movementTrail != null)
        {
            var emission = movementTrail.emission;
            emission.rateOverTime = Mathf.Lerp(10f, 100f, SpeedPercentage);
            
            var velocityOverLifetime = movementTrail.velocityOverLifetime;
            velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
            velocityOverLifetime.enabled = true;
        }
    }
    
    private void CheckLightSpeedThreshold()
    {
        if (!isLightSpeedReached && currentSpeed >= maxSpeed * 0.99f)
        {
            ReachLightSpeed();
        }
    }
    
    private void ReachLightSpeed()
    {
        isLightSpeedReached = true;
        
        // Stop movement
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
        
        // Play effects
        if (audioSource != null && speedUpSound != null)
            audioSource.PlayOneShot(speedUpSound);
        
        // Notify systems
        OnLightSpeedReached?.Invoke();
        
        // Trigger cutscene
        if (cutsceneManager != null)
        {
            cutsceneManager.PlayLightSpeedCutscene();
        }
        
        Debug.Log("Light speed reached! Triggering cutscene...");
    }
    
    private void HandlePowerCollected(float amount)
    {
        // Play collection effects
        if (powerCollectionEffect != null)
        {
            powerCollectionEffect.Play();
        }
        
        if (audioSource != null && powerCollectSound != null)
        {
            audioSource.PlayOneShot(powerCollectSound);
        }
    }
    
    private void HandlePowerLevelChanged(float newPowerLevel)
    {
        powerLevel = newPowerLevel;
        OnPowerChanged?.Invoke(powerLevel);
    }
    
    // Public methods for other systems
    public void AddPower(float amount)
    {
        if (powerSystem != null)
            powerSystem.AddPower(amount);
    }
    
    public void SetAutoMove(bool enabled)
    {
        autoMove = enabled;
    }
    
    public float GetStrength()
    {
        return 1f + (powerLevel * powerToStrengthMultiplier);
    }
    
    public void ResetPlayer()
    {
        powerLevel = 0f;
        currentSpeed = baseSpeed;
        isLightSpeedReached = false;
        movementDirection = Vector3.forward;
        
        if (powerSystem != null)
            powerSystem.ResetPower();
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw obstacle detection rays
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, movementDirection * obstacleDetectionDistance);
        
        // Draw left and right detection rays
        if (movementDirection != Vector3.zero)
        {
            Vector3 left = Vector3.Cross(movementDirection, Vector3.up).normalized;
            Vector3 right = -left;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, left * obstacleDetectionDistance);
            Gizmos.DrawRay(transform.position, right * obstacleDetectionDistance);
        }
        
        // Draw ground check sphere
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
