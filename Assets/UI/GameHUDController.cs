using UnityEngine;
using UnityEngine.UIElements;

public class GameHUDController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private UIDocument hudDocument;
    [SerializeField] private UIDocument inventoryDocument;
    [SerializeField] private UIDocument questTrackerDocument;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;

    private Button pauseButton;
    private Button dpadUp;
    private Button dpadDown;
    private Button dpadLeft;
    private Button dpadRight;

    [Header("Action Buttons")]
    private Button actionButtonA;
    private Button actionButtonB;
    private Button actionButtonX;
    private Button actionButtonY;

    private Label fpsLabel;
    private VisualElement mobileControls;

    // Health UI
    private VisualElement healthBarFill;
    private Label healthText;
    
    // Stats UI
    private Label levelText;
    private Label attackText;
    private Label defenseText;
    private Label speedText;
    private Label currencyText;
    
    // Inventory UI
    private Button inventoryButton;
    private VisualElement inventoryOverlay;
    private Label slot1Icon, slot2Icon, slot3Icon;
    
    // Quest Tracker UI
    private Button questLogButton;
    private VisualElement questTrackerOverlay;
    private Label questTitle;
    private Label questDescription;
    private VisualElement questProgressFill;
    private Label questProgressText;

    private bool isPaused = false;
    private bool isInventoryOpen = false;
    private bool isQuestTrackerOpen = false;
    private PlayerInput playerInput;
    private Health playerHealth;
    private PlayerStats playerStats;
    private PlayerInventory playerInventory;

    private void OnEnable()
    {
        playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput not found in scene!");
            return;
        }
        
        // Find player components
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            playerStats = player.GetComponent<PlayerStats>();
            playerInventory = player.GetComponent<PlayerInventory>();
        }
        
        
        var root = hudDocument.rootVisualElement;

        // Initialize UI elements
        pauseButton = root.Q<Button>("pause-button");
        dpadUp = root.Q<Button>("dpad-up");
        dpadDown = root.Q<Button>("dpad-down");
        dpadLeft = root.Q<Button>("dpad-left");
        dpadRight = root.Q<Button>("dpad-right");

        actionButtonA = root.Q<Button>("action-button-a");
        actionButtonB = root.Q<Button>("action-button-b");
        actionButtonX = root.Q<Button>("action-button-x");
        actionButtonY = root.Q<Button>("action-button-y");
        
        fpsLabel = root.Q<Label>("fps-counter");
        mobileControls = root.Q<VisualElement>("mobile-controls");
        
        // Initialize Health UI
        healthBarFill = root.Q<VisualElement>("health-bar-fill");
        healthText = root.Q<Label>("health-text");
        
        // Initialize Stats UI
        levelText = root.Q<Label>("level-text");
        attackText = root.Q<Label>("attack-text");
        defenseText = root.Q<Label>("defense-text");
        speedText = root.Q<Label>("speed-text");
        currencyText = root.Q<Label>("currency-text");
        
        // Initialize Inventory UI
        inventoryButton = root.Q<Button>("inventory-button");
        inventoryOverlay = root.Q<VisualElement>("inventory-overlay");
        slot1Icon = root.Q<Label>("slot-1-icon");
        slot2Icon = root.Q<Label>("slot-2-icon");
        slot3Icon = root.Q<Label>("slot-3-icon");
        
        // Initialize Quest Tracker UI
        questLogButton = root.Q<Button>("quest-log-button");
        questTrackerOverlay = root.Q<VisualElement>("quest-tracker-overlay");
        questTitle = root.Q<Label>("quest-title");
        questDescription = root.Q<Label>("quest-description");
        questProgressFill = root.Q<VisualElement>("quest-progress-fill");
        questProgressText = root.Q<Label>("quest-progress-text");

        // Register button callbacks
        // if (pauseButton != null)
            pauseButton.clicked += TogglePause;
        
        // Movement buttons - use mouse events for continuous input
        if (dpadUp != null)
            RegisterMovementButton(dpadUp, () => playerInput.SetHudUp(true), () => playerInput.SetHudUp(false));
        if (dpadLeft != null)
            RegisterMovementButton(dpadLeft, () => playerInput.SetHudMoveLeft(true), () => playerInput.SetHudMoveLeft(false));
        if (dpadRight != null)
            RegisterMovementButton(dpadRight, () => playerInput.SetHudMoveRight(true), () => playerInput.SetHudMoveRight(false));
        if (dpadDown != null)
            dpadDown.clicked += () => Debug.Log("DPad Down Pressed"); // Handle if needed

        // Action buttons - use click events for one-shot actions
        if (actionButtonA != null)
            actionButtonA.clicked += () => {
                Debug.Log("Action Button A (Jump) clicked");
                playerInput.TriggerHudJump();
            };
        if (actionButtonB != null)
            actionButtonB.clicked += () => {
                Debug.Log("Action Button B (Dash) clicked");
                playerInput.TriggerHudDash();
            };
        if (actionButtonX != null)
            actionButtonX.clicked += () => {
                Debug.Log("Action Button X (Attack) clicked");
                playerInput.TriggerHudAttack();
            };
        if (actionButtonY != null)
            actionButtonY.clicked += () => {
                Debug.Log("Action Button Y (Generic Action) clicked");
                // Generic action - can be customized
            };
        // Inventory button
        if (inventoryButton != null)
            inventoryButton.clicked += ToggleInventory;
        
        // Quest log button
        if (questLogButton != null)
            questLogButton.clicked += ToggleQuestTracker;
        
        // Initialize mobile controls visibility
        UpdateMobileControlsVisibility();
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
        UpdateMobileControlsVisibility();
        Debug.Log("Game paused");
    }

    private void Update()
    {
        HandleKeyboardAndJoystickInput();
        UpdateFPSDisplay();
        UpdateHealthBar();
        UpdateStatsDisplay();
        UpdateInventoryDisplay();
        UpdateQuestTracker();
    }

    private void HandleKeyboardAndJoystickInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
        
        if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.Q))
        {
            ToggleQuestTracker();
        }
        
        // Generic hotkeys (can be customized)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Alpha1 pressed - generic action");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Alpha2 pressed - generic action");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Alpha3 pressed - generic action");
        }

        // Add additional keyboard/joystick input handling here
    }

    private void UpdateFPSDisplay()
    {
        float fps = 1 / Time.unscaledDeltaTime;
        fpsLabel.text = $"FPS: {Mathf.RoundToInt(fps)}";
    }

    private void UpdateMobileControlsVisibility()
    {
        // Check if mobile controls should be visible
        bool shouldShowMobileControls = ShouldShowMobileControls();
        mobileControls.style.display = shouldShowMobileControls ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private bool ShouldShowMobileControls()
    {
        // Hide mobile controls if game is paused
        if (isPaused) return false;
        
        // Check settings for virtual joystick preference
        var settings = SettingsData.Load();
        return settings.virtualJoystick != "None";
    }

    // Public method to be called by pause menu when it resumes the game
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        UpdateMobileControlsVisibility();
        Debug.Log("Game resumed");
    }

    // Public method to sync pause state (in case pause menu resumes from elsewhere)
    public void SyncPauseState()
    {
        isPaused = pauseMenu.activeInHierarchy;
        UpdateMobileControlsVisibility();
    }

    // Public method to be called when settings are applied
    public void OnSettingsApplied()
    {
        UpdateMobileControlsVisibility();
    }

    // Helper method to register movement button with press/release events
    private void RegisterMovementButton(Button button, System.Action onPressed, System.Action onReleased)
    {
        // Capture phase ensures we receive the PointerDown before Button's Clickable
        button.RegisterCallback<PointerDownEvent>(evt => {
            onPressed();
            evt.StopImmediatePropagation();
        }, TrickleDown.TrickleDown);

        // PointerUp (capture) to release; also cancel on PointerCancel or Leave
        button.RegisterCallback<PointerUpEvent>(evt => {
            onReleased();
            evt.StopImmediatePropagation();
        }, TrickleDown.TrickleDown);

        button.RegisterCallback<PointerCancelEvent>(evt => onReleased(), TrickleDown.TrickleDown);
        button.RegisterCallback<PointerLeaveEvent>(evt => onReleased(), TrickleDown.TrickleDown);
    }
    
    // Health Bar Update
    private void UpdateHealthBar()
    {
        if (playerHealth != null && healthBarFill != null && healthText != null)
        {
            float healthPercentage = playerHealth.CurrentHealth / playerHealth.MaxHealth;
            healthBarFill.style.width = Length.Percent(healthPercentage * 100);
            healthText.text = $"{Mathf.RoundToInt(playerHealth.CurrentHealth)}/{Mathf.RoundToInt(playerHealth.MaxHealth)}";
            
            // Change color based on health percentage
            if (healthPercentage > 0.6f)
                healthBarFill.style.backgroundColor = new Color(0.2f, 1f, 0.2f); // Green
            else if (healthPercentage > 0.3f)
                healthBarFill.style.backgroundColor = new Color(1f, 1f, 0.2f); // Yellow
            else
                healthBarFill.style.backgroundColor = new Color(1f, 0.2f, 0.2f); // Red
        }
    }
    
    // Stats Display Update
    private void UpdateStatsDisplay()
    {
        if (playerStats != null)
        {
            if (levelText != null)
                levelText.text = $"Level: {playerStats.GetStat(StatType.Health) / 10}"; // Approximation
            if (attackText != null)
                attackText.text = $"ATK: {playerStats.GetStat(StatType.Attack)}";
            if (defenseText != null)
                defenseText.text = $"DEF: {playerStats.GetStat(StatType.Defense)}";
            if (speedText != null)
                speedText.text = $"SPD: {playerStats.GetStat(StatType.Speed)}";
        }
        
        if (playerInventory != null && currencyText != null)
        {
            currencyText.text = $"Gold: {playerInventory.GetCurrency()}";
        }
    }
    
    // Inventory Display Update
    private void UpdateInventoryDisplay()
    {
        if (playerInventory != null)
        {
            // Update inventory slot icons (for generic items)
            if (slot1Icon != null)
                slot1Icon.style.color = Color.white;
            if (slot2Icon != null)
                slot2Icon.style.color = Color.white;
            if (slot3Icon != null)
                slot3Icon.style.color = Color.white;
        }
    }
    
    // Quest Tracker Update
    private void UpdateQuestTracker()
    {
        // Quest system removed - placeholder for future implementation
        if (questTitle != null)
            questTitle.text = "No Active Quest";
        if (questDescription != null)
            questDescription.text = "No active quest";
        if (questProgressFill != null)
            questProgressFill.style.width = Length.Percent(0);
        if (questProgressText != null)
            questProgressText.text = "0/0";
    }
    
    // Toggle Inventory
    private void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        
        if (inventoryOverlay != null)
        {
            inventoryOverlay.style.display = isInventoryOpen ? DisplayStyle.Flex : DisplayStyle.None;
            
            if (isInventoryOpen && inventoryDocument != null)
            {
                // Load inventory UI into overlay
                inventoryOverlay.Clear();
                var inventoryRoot = inventoryDocument.rootVisualElement.Q("inventory-container");
                if (inventoryRoot != null)
                {
                    // Clone the element to avoid moving it from the original document
                    var clonedInventory = inventoryRoot.Q(null); // This gets the first child
                    if (clonedInventory != null)
                    {
                        inventoryOverlay.Add(clonedInventory);
                    }
                    else
                    {
                        // If cloning fails, add the original (this might move it)
                        inventoryOverlay.Add(inventoryRoot);
                    }
                }
            }
        }
        
        // Pause game when inventory is open (but not if quest tracker is also open)
        if (isInventoryOpen || isQuestTrackerOpen)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }
    
    // Toggle Quest Tracker
    private void ToggleQuestTracker()
    {
        isQuestTrackerOpen = !isQuestTrackerOpen;
        
        if (questTrackerOverlay != null)
        {
            questTrackerOverlay.style.display = isQuestTrackerOpen ? DisplayStyle.Flex : DisplayStyle.None;
            
            if (isQuestTrackerOpen && questTrackerDocument != null)
            {
                // Load quest tracker UI into overlay
                questTrackerOverlay.Clear();
                var questTrackerRoot = questTrackerDocument.rootVisualElement.Q("quest-tracker-container");
                if (questTrackerRoot != null)
                {
                    // Clone the element to avoid moving it from the original document
                    var clonedQuestTracker = questTrackerRoot.Q(null); // This gets the first child
                    if (clonedQuestTracker != null)
                    {
                        questTrackerOverlay.Add(clonedQuestTracker);
                    }
                    else
                    {
                        // If cloning fails, add the original (this might move it)
                        questTrackerOverlay.Add(questTrackerRoot);
                    }
                }
            }
        }
        
        // Pause game when quest tracker is open (but not if inventory is also open)
        if (isInventoryOpen || isQuestTrackerOpen)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }
    
    // Helper methods for quest system - removed for reimplementation
}

