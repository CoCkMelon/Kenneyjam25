using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Memory Pattern Puzzle - Remember and recreate sequences of power activations
/// Theme: Master the sleight of hand by remembering and repeating power patterns
/// </summary>
public class MemoryPatternPuzzle : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private int startingPatternLength = 3;
    [SerializeField] private int maxPatternLength = 8;
    [SerializeField] private float displaySpeed = 0.8f;
    [SerializeField] private float inputTimeLimit = 10f;
    [SerializeField] private bool allowReplay = true;
    [SerializeField] private int maxReplayAttempts = 2;
    
    [Header("Pattern Elements")]
    [SerializeField] private PatternElement[] patternElements;
    [SerializeField] private AudioClip[] sequenceSounds;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem successEffect;
    [SerializeField] private ParticleSystem failureEffect;
    [SerializeField] private Material activatedMaterial;
    [SerializeField] private Material deactivatedMaterial;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject replayButton;
    [SerializeField] private UnityEngine.UI.Slider progressSlider;
    [SerializeField] private UnityEngine.UI.Text statusText;
    
    [Header("Events")]
    public UnityEvent OnPuzzleComplete;
    public UnityEvent OnPuzzleStart;
    public UnityEvent OnPatternCorrect;
    public UnityEvent OnPatternIncorrect;
    
    // Private fields
    private List<int> currentPattern = new List<int>();
    private List<int> playerInput = new List<int>();
    private int currentPatternLength;
    private bool isDisplayingPattern = false;
    private bool isWaitingForInput = false;
    private bool isPuzzleCompleted = false;
    private float inputTimer = 0f;
    private int replayAttempts = 0;
    private AudioSource audioSource;
    private Camera puzzleCamera;
    
    // Properties
    public bool IsPuzzleCompleted => isPuzzleCompleted;
    public int CurrentPatternLength => currentPatternLength;
    public bool IsWaitingForInput => isWaitingForInput;
    
    void Start()
    {
        puzzleCamera = Camera.main;
        if (puzzleCamera == null)
            puzzleCamera = FindFirstObjectByType<Camera>();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        InitializePuzzle();
    }
    
    void Update()
    {
        HandleInput();
        UpdateTimer();
        UpdateUI();
    }
    
    private void InitializePuzzle()
    {
        currentPatternLength = startingPatternLength;
        
        // Initialize pattern elements
        for (int i = 0; i < patternElements.Length; i++)
        {
            patternElements[i].Initialize(this, i);
        }
        
        // Setup UI
        if (startButton != null)
        {
            var button = startButton.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
                button.onClick.AddListener(StartPuzzle);
        }
        
        if (replayButton != null)
        {
            var button = replayButton.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
                button.onClick.AddListener(ReplayPattern);
            replayButton.SetActive(false);
        }
        
        ResetPuzzle();
    }
    
    private void HandleInput()
    {
        if (!isWaitingForInput || isPuzzleCompleted) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = puzzleCamera.ScreenPointToRay(mousePos);
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                PatternElement element = hit.collider.GetComponent<PatternElement>();
                if (element != null)
                {
                    HandleElementClick(element.ElementIndex);
                }
            }
        }
    }
    
    private void UpdateTimer()
    {
        if (isWaitingForInput && !isPuzzleCompleted)
        {
            inputTimer -= Time.deltaTime;
            
            if (inputTimer <= 0f)
            {
                // Time's up!
                OnInputTimeUp();
            }
        }
    }
    
    private void UpdateUI()
    {
        if (progressSlider != null)
        {
            if (isWaitingForInput)
            {
                progressSlider.value = inputTimer / inputTimeLimit;
            }
            else
            {
                progressSlider.value = 1f;
            }
        }
        
        if (statusText != null)
        {
            if (isPuzzleCompleted)
            {
                statusText.text = "Puzzle Complete!";
            }
            else if (isDisplayingPattern)
            {
                statusText.text = "Watch the pattern...";
            }
            else if (isWaitingForInput)
            {
                statusText.text = $"Repeat the pattern ({playerInput.Count}/{currentPattern.Count})";
            }
            else
            {
                statusText.text = "Click Start to begin";
            }
        }
    }
    
    public void StartPuzzle()
    {
        if (isDisplayingPattern || isWaitingForInput) return;
        
        OnPuzzleStart?.Invoke();
        StartCoroutine(DisplayPattern());
    }
    
    public void ReplayPattern()
    {
        if (replayAttempts >= maxReplayAttempts || !allowReplay) return;
        
        replayAttempts++;
        playerInput.Clear();
        StartCoroutine(DisplayPattern());
        
        if (replayButton != null)
        {
            replayButton.SetActive(replayAttempts < maxReplayAttempts);
        }
    }
    
    private IEnumerator DisplayPattern()
    {
        isDisplayingPattern = true;
        isWaitingForInput = false;
        
        // Generate new pattern if needed
        if (currentPattern.Count == 0)
        {
            GeneratePattern();
        }
        
        // Display each element in the pattern
        for (int i = 0; i < currentPattern.Count; i++)
        {
            int elementIndex = currentPattern[i];
            
            // Activate element
            patternElements[elementIndex].Activate();
            
            // Play sound
            if (sequenceSounds != null && elementIndex < sequenceSounds.Length)
            {
                audioSource.PlayOneShot(sequenceSounds[elementIndex]);
            }
            
            yield return new WaitForSeconds(displaySpeed);
            
            // Deactivate element
            patternElements[elementIndex].Deactivate();
            
            yield return new WaitForSeconds(displaySpeed * 0.5f);
        }
        
        // Start input phase
        isDisplayingPattern = false;
        isWaitingForInput = true;
        inputTimer = inputTimeLimit;
        
        if (replayButton != null && allowReplay)
        {
            replayButton.SetActive(true);
        }
    }
    
    private void GeneratePattern()
    {
        currentPattern.Clear();
        
        for (int i = 0; i < currentPatternLength; i++)
        {
            int randomIndex = Random.Range(0, patternElements.Length);
            currentPattern.Add(randomIndex);
        }
    }
    
    private void HandleElementClick(int elementIndex)
    {
        if (!isWaitingForInput) return;
        
        playerInput.Add(elementIndex);
        
        // Activate element briefly
        patternElements[elementIndex].Activate();
        StartCoroutine(DeactivateElementAfterDelay(elementIndex, 0.3f));
        
        // Play sound
        if (sequenceSounds != null && elementIndex < sequenceSounds.Length)
        {
            audioSource.PlayOneShot(sequenceSounds[elementIndex]);
        }
        
        // Check if input is correct so far
        if (playerInput.Count <= currentPattern.Count)
        {
            bool isCorrect = true;
            for (int i = 0; i < playerInput.Count; i++)
            {
                if (playerInput[i] != currentPattern[i])
                {
                    isCorrect = false;
                    break;
                }
            }
            
            if (!isCorrect)
            {
                OnIncorrectInput();
            }
            else if (playerInput.Count == currentPattern.Count)
            {
                OnCorrectPattern();
            }
        }
    }
    
    private IEnumerator DeactivateElementAfterDelay(int elementIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        patternElements[elementIndex].Deactivate();
    }
    
    private void OnCorrectPattern()
    {
        isWaitingForInput = false;
        
        // Play success sound
        if (audioSource != null && correctSound != null)
            audioSource.PlayOneShot(correctSound);
        
        // Play success effect
        if (successEffect != null)
            successEffect.Play();
        
        OnPatternCorrect?.Invoke();
        
        // Check if puzzle is complete
        if (currentPatternLength >= maxPatternLength)
        {
            CompletePuzzle();
        }
        else
        {
            // Increase difficulty
            currentPatternLength++;
            StartCoroutine(NextRoundDelay());
        }
    }
    
    private void OnIncorrectInput()
    {
        isWaitingForInput = false;
        
        // Play failure sound
        if (audioSource != null && incorrectSound != null)
            audioSource.PlayOneShot(incorrectSound);
        
        // Play failure effect
        if (failureEffect != null)
            failureEffect.Play();
        
        OnPatternIncorrect?.Invoke();
        
        // Flash all elements red
        StartCoroutine(FlashElementsRed());
        
        // Reset for retry
        StartCoroutine(RetryDelay());
    }
    
    private void OnInputTimeUp()
    {
        OnIncorrectInput();
    }
    
    private IEnumerator NextRoundDelay()
    {
        yield return new WaitForSeconds(1.5f);
        
        // Reset for next round
        currentPattern.Clear();
        playerInput.Clear();
        replayAttempts = 0;
        
        if (replayButton != null)
            replayButton.SetActive(false);
        
        StartCoroutine(DisplayPattern());
    }
    
    private IEnumerator RetryDelay()
    {
        yield return new WaitForSeconds(1.5f);
        
        // Reset for retry
        playerInput.Clear();
        StartCoroutine(DisplayPattern());
    }
    
    private IEnumerator FlashElementsRed()
    {
        // Flash all elements red briefly
        foreach (PatternElement element in patternElements)
        {
            element.SetErrorState(true);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        foreach (PatternElement element in patternElements)
        {
            element.SetErrorState(false);
        }
    }
    
    private void CompletePuzzle()
    {
        isPuzzleCompleted = true;
        isWaitingForInput = false;
        
        // Play completion effects
        if (successEffect != null)
            successEffect.Play();
        
        // Activate all elements in celebration
        foreach (PatternElement element in patternElements)
        {
            element.SetCompleted(true);
        }
        
        OnPuzzleComplete?.Invoke();
        
        Debug.Log("Memory Pattern Puzzle Completed!");
    }
    
    public void ResetPuzzle()
    {
        isPuzzleCompleted = false;
        isDisplayingPattern = false;
        isWaitingForInput = false;
        currentPatternLength = startingPatternLength;
        replayAttempts = 0;
        
        currentPattern.Clear();
        playerInput.Clear();
        
        // Reset all elements
        foreach (PatternElement element in patternElements)
        {
            element.Reset();
        }
        
        if (replayButton != null)
            replayButton.SetActive(false);
    }
    
    // Public methods for external control
    public void SetPatternLength(int length)
    {
        currentPatternLength = Mathf.Clamp(length, 1, maxPatternLength);
    }
    
    public void SetDisplaySpeed(float speed)
    {
        displaySpeed = Mathf.Max(0.1f, speed);
    }
    
    public void SetInputTimeLimit(float timeLimit)
    {
        inputTimeLimit = Mathf.Max(1f, timeLimit);
    }
}
