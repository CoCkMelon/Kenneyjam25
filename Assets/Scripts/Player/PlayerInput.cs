using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Input Settings")]
    public bool enableMobileControls = true;
    public bool enableKeyboardControls = true;
    public bool enableMouseLook = true;
    
    [Header("Mobile Virtual Joystick")]
    public VirtualJoystick moveJoystick;
    public VirtualJoystick lookJoystick;
    
    // Movement input states
    private bool hudUp = false;
    private bool hudDown = false;
    private bool hudLeft = false;
    private bool hudRight = false;
    private bool hudMoveLeft = false;
    private bool hudMoveRight = false;
    
    // Action input states
    private bool hudJump = false;
    private bool hudDash = false;
    private bool hudAttack = false;
    
    // Input values
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool AttackInput { get; private set; }
    public bool RunInput { get; private set; }
    
    void Update()
    {
        UpdateMovementInput();
        UpdateLookInput();
        UpdateActionInput();
    }
    
    private void UpdateMovementInput()
    {
        Vector2 moveInput = Vector2.zero;
        
        // Handle keyboard input
        if (enableKeyboardControls)
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || hudUp)
                moveInput.y += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || hudDown)
                moveInput.y -= 1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || hudLeft || hudMoveLeft)
                moveInput.x -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || hudRight || hudMoveRight)
                moveInput.x += 1f;
        }
        
        // Handle mobile joystick input
        if (enableMobileControls && moveJoystick != null)
        {
            Vector2 joystickInput = moveJoystick.GetInputVector();
            moveInput += joystickInput;
        }
        
        // Handle Unity's built-in input system
        if (enableKeyboardControls)
        {
            moveInput.x += Input.GetAxis("Horizontal");
            moveInput.y += Input.GetAxis("Vertical");
        }
        
        // Normalize diagonal movement
        if (moveInput.magnitude > 1f)
            moveInput.Normalize();
            
        MoveInput = moveInput;
    }
    
    private void UpdateLookInput()
    {
        Vector2 lookInput = Vector2.zero;
        
        // Handle mouse input
        if (enableMouseLook && !Application.isMobilePlatform)
        {
            lookInput.x = Input.GetAxis("Mouse X");
            lookInput.y = Input.GetAxis("Mouse Y");
        }
        
        // Handle mobile joystick input
        if (enableMobileControls && lookJoystick != null)
        {
            Vector2 joystickInput = lookJoystick.GetInputVector();
            lookInput += joystickInput;
        }
        
        LookInput = lookInput;
    }
    
    private void UpdateActionInput()
    {
        // Jump input
        JumpInput = Input.GetButtonDown("Jump") || hudJump;
        if (hudJump) hudJump = false; // Reset one-shot actions
        
        // Dash input
        DashInput = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || hudDash;
        if (hudDash) hudDash = false;
        
        // Attack input
        AttackInput = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || hudAttack;
        if (hudAttack) hudAttack = false;
        
        // Run input (hold)
        RunInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }
    
    // HUD Input Methods (called by GameHUDController)
    public void SetHudUp(bool pressed) { hudUp = pressed; }
    public void SetHudDown(bool pressed) { hudDown = pressed; }
    public void SetHudLeft(bool pressed) { hudLeft = pressed; }
    public void SetHudRight(bool pressed) { hudRight = pressed; }
    public void SetHudMoveLeft(bool pressed) { hudMoveLeft = pressed; }
    public void SetHudMoveRight(bool pressed) { hudMoveRight = pressed; }
    
    public void TriggerHudJump() { hudJump = true; }
    public void TriggerHudDash() { hudDash = true; }
    public void TriggerHudAttack() { hudAttack = true; }
}
