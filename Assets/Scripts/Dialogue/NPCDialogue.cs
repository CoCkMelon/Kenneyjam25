using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("NPC Settings")]
    public string npcName = "NPC";
    
    [Header("Dialogue Source")]
    public string dialogueScenePath;
    
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public Vector3 bubbleOffset = new Vector3(0, 2f, 0);
    
    [Header("Animation Settings")]
    public Animator npcAnimator;
    public string idleAnimation = "idle";
    public string talkingAnimation = "talking";
    
    [Header("Speech Bubble Settings")]
    public bool showSpeechBubble = true;
    public string greetingText = "Hello there!";
    public float bubbleDisplayTime = 2f;
    
    private Transform player;
    private DialogueUIController uiController;
    private bool isPlayerNear = false;
    private bool isDialogueActive = false;
    private float bubbleTimer = 0f;
    
    void Start()
    {
        // Find player
        var playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            player = playerController.transform;
        }
        
        // Find UI controller
        uiController = DialogueUIController.Instance;
        if (uiController == null)
        {
            Debug.LogError("DialogueUIController not found! Please add it to the scene.");
        }
    }
    
    void Update()
    {
        if (player == null || uiController == null) return;
        
        float distance = Vector3.Distance(transform.position, player.position);
        bool shouldShowPrompt = distance <= interactionDistance && !isDialogueActive;
        
        // Handle interaction prompt
        if (shouldShowPrompt && !isPlayerNear)
        {
            isPlayerNear = true;
            uiController.ShowInteractionPrompt(transform.position + bubbleOffset);
            
            // Show speech bubble greeting
            if (showSpeechBubble)
            {
                uiController.ShowSpeechBubble(greetingText, transform, bubbleOffset);
                bubbleTimer = bubbleDisplayTime;
            }
        }
        else if (!shouldShowPrompt && isPlayerNear)
        {
            isPlayerNear = false;
            uiController.HideInteractionPrompt();
            uiController.HideSpeechBubble();
        }
        
        // Update bubble timer
        if (bubbleTimer > 0)
        {
            bubbleTimer -= Time.deltaTime;
            if (bubbleTimer <= 0)
            {
                uiController.HideSpeechBubble();
            }
        }
        
        // Check for interaction input
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogue();
        }
    }
    
    public void StartDialogue()
    {
        if (string.IsNullOrEmpty(dialogueScenePath))
        {
            Debug.LogWarning($"No dialogue scene path set for NPC: {npcName}");
            return;
        }
        
        isDialogueActive = true;
        uiController.HideInteractionPrompt();
        uiController.HideSpeechBubble();
        
        // Set NPC animation to talking
        if (npcAnimator != null)
        {
            npcAnimator.Play(talkingAnimation);
        }
        
        // Start dialogue from YAML file
        DialogueManager.Instance.LoadAndStartScene(dialogueScenePath);
    }
    
    public void EndDialogue()
    {
        isDialogueActive = false;
        
        // Set NPC animation back to idle
        if (npcAnimator != null)
        {
            npcAnimator.Play(idleAnimation);
        }
    }
    
    // Called when dialogue ends (this should be called from DialogueManager)
    void OnDialogueEnd()
    {
        EndDialogue();
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw interaction range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
        
        // Draw speech bubble position
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + bubbleOffset, Vector3.one * 0.5f);
    }
}
