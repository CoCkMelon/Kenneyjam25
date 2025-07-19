using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Single-script sleight puzzle using MenuButton3D components
/// Grid Logic Puzzle: Arrange buttons to satisfy magical power constraints
/// Uses constraints validated by Z3/MiniZinc during development
/// </summary>
public class SleightGridPuzzle : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] private int gridSize = 3;
    [SerializeField] private int requiredActivePerRow = 2;
    [SerializeField] private int requiredActivePerColumn = 2;
    [SerializeField] private int minActiveCorners = 1;
    [SerializeField] private bool showSolution = false;
    
    [Header("Grid Layout")]
    [SerializeField] private float buttonSpacing = 2f;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform gridParent;
    
    [Header("Visual Feedback")]
    [SerializeField] private Material inactiveButtonMaterial;
    [SerializeField] private Material activeButtonMaterial;
    [SerializeField] private Material correctButtonMaterial;
    [SerializeField] private Material incorrectButtonMaterial;
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failureSound;
    
    [Header("Events")]
    public UnityEvent OnPuzzleSolved;
    public UnityEvent OnPuzzleFailed;
    public UnityEvent OnButtonClicked;
    
    // Puzzle state
    private bool[,] buttonStates;
    private bool[,] solutionGrid;
    private GameObject[,] buttonObjects;
    private bool isPuzzleSolved = false;
    private bool isCheckingConstraints = false;
    
    // Valid solution (pre-computed from Z3/MiniZinc)
    // This is one of the valid solutions found by the constraint solver
    private readonly bool[,] validSolution = new bool[,]
    {
        {true, true, false},
        {false, true, true},
        {true, false, true}
    };
    
    void Start()
    {
        InitializePuzzle();
    }
    
    private void InitializePuzzle()
    {
        // Initialize grid arrays
        buttonStates = new bool[gridSize, gridSize];
        solutionGrid = new bool[gridSize, gridSize];
        buttonObjects = new GameObject[gridSize, gridSize];
        
        // Copy valid solution
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                solutionGrid[i, j] = validSolution[i, j];
            }
        }
        
        // Create button grid
        CreateButtonGrid();
        
        // Randomize initial state
        RandomizeButtons();
        
        // Update visuals
        UpdateAllButtonVisuals();
        
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }
    
    private void CreateButtonGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                // Calculate position
                Vector3 position = new Vector3(
                    (j - gridSize / 2f) * buttonSpacing,
                    0,
                    (i - gridSize / 2f) * buttonSpacing
                );
                
                // Create button object
                GameObject button = Instantiate(buttonPrefab, gridParent);
                button.transform.localPosition = position;
                button.name = $"Button_{i}_{j}";
                
                // Add MenuButton3D component if not present
                MenuButton3D menuButton = button.GetComponent<MenuButton3D>();
                if (menuButton == null)
                {
                    menuButton = button.AddComponent<MenuButton3D>();
                }
                
                // Create custom action for this button
                int row = i, col = j; // Capture for closure
                
                // Add click handler using OnMouseDown
                SleightButtonClickHandler clickHandler = button.GetComponent<SleightButtonClickHandler>();
                if (clickHandler == null)
                {
                    clickHandler = button.AddComponent<SleightButtonClickHandler>();
                }
                clickHandler.Initialize(this, row, col);
                
                buttonObjects[i, j] = button;
            }
        }
    }
    
    private void RandomizeButtons()
    {
        // Start with all buttons off
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                buttonStates[i, j] = false;
            }
        }
        
        // Randomly activate some buttons (but not the solution)
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (Random.value < 0.3f) // 30% chance to be active initially
                {
                    buttonStates[i, j] = true;
                }
            }
        }
    }
    
    public void HandleButtonClick(int row, int col)
    {
        if (isPuzzleSolved || isCheckingConstraints) return;
        
        // Toggle button state
        buttonStates[row, col] = !buttonStates[row, col];
        
        // Play sound
        if (audioSource != null && buttonClickSound != null)
            audioSource.PlayOneShot(buttonClickSound);
        
        // Update visuals
        UpdateButtonVisual(row, col);
        
        // Check if puzzle is solved
        CheckPuzzleState();
        
        OnButtonClicked?.Invoke();
    }
    
    private void UpdateButtonVisual(int row, int col)
    {
        GameObject button = buttonObjects[row, col];
        Renderer renderer = button.GetComponent<Renderer>();
        
        if (renderer != null)
        {
            if (showSolution)
            {
                // Show solution colors
                if (solutionGrid[row, col])
                {
                    renderer.material = buttonStates[row, col] ? correctButtonMaterial : incorrectButtonMaterial;
                }
                else
                {
                    renderer.material = buttonStates[row, col] ? incorrectButtonMaterial : correctButtonMaterial;
                }
            }
            else
            {
                // Show normal active/inactive colors
                renderer.material = buttonStates[row, col] ? activeButtonMaterial : inactiveButtonMaterial;
            }
        }
    }
    
    private void UpdateAllButtonVisuals()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                UpdateButtonVisual(i, j);
            }
        }
    }
    
    private void CheckPuzzleState()
    {
        isCheckingConstraints = true;
        
        bool satisfiesConstraints = CheckAllConstraints();
        
        if (satisfiesConstraints)
        {
            // Puzzle solved!
            isPuzzleSolved = true;
            
            // Play success sound
            if (audioSource != null && successSound != null)
                audioSource.PlayOneShot(successSound);
            
            // Show solution colors
            showSolution = true;
            UpdateAllButtonVisuals();
            
            OnPuzzleSolved?.Invoke();
            
            Debug.Log("Sleight Grid Puzzle Solved!");
        }
        
        isCheckingConstraints = false;
    }
    
    private bool CheckAllConstraints()
    {
        // Constraint 1: Each row must have exactly requiredActivePerRow active buttons
        for (int i = 0; i < gridSize; i++)
        {
            int rowSum = 0;
            for (int j = 0; j < gridSize; j++)
            {
                if (buttonStates[i, j]) rowSum++;
            }
            if (rowSum != requiredActivePerRow) return false;
        }
        
        // Constraint 2: Each column must have exactly requiredActivePerColumn active buttons
        for (int j = 0; j < gridSize; j++)
        {
            int colSum = 0;
            for (int i = 0; i < gridSize; i++)
            {
                if (buttonStates[i, j]) colSum++;
            }
            if (colSum != requiredActivePerColumn) return false;
        }
        
        // Constraint 3: At least minActiveCorners corners must be active
        int cornerSum = 0;
        int[] corners = {0, 0, 0, 2, 2, 0, 2, 2}; // (0,0), (0,2), (2,0), (2,2)
        for (int i = 0; i < 4; i++)
        {
            int row = corners[i * 2];
            int col = corners[i * 2 + 1];
            if (buttonStates[row, col]) cornerSum++;
        }
        if (cornerSum < minActiveCorners) return false;
        
        return true;
    }
    
    public void ResetPuzzle()
    {
        isPuzzleSolved = false;
        isCheckingConstraints = false;
        showSolution = false;
        
        RandomizeButtons();
        UpdateAllButtonVisuals();
    }
    
    public void ShowSolution()
    {
        showSolution = !showSolution;
        UpdateAllButtonVisuals();
    }
    
    public void SetSolutionState()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                buttonStates[i, j] = solutionGrid[i, j];
            }
        }
        UpdateAllButtonVisuals();
        CheckPuzzleState();
    }
    
    // Public getters for external systems
    public bool IsPuzzleSolved => isPuzzleSolved;
    public float CompletionPercentage
    {
        get
        {
            int correctButtons = 0;
            int totalButtons = gridSize * gridSize;
            
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    if (buttonStates[i, j] == solutionGrid[i, j])
                        correctButtons++;
                }
            }
            
            return (float)correctButtons / totalButtons;
        }
    }
}

/// <summary>
/// Simple click handler component for individual buttons
/// </summary>
public class SleightButtonClickHandler : MonoBehaviour
{
    private SleightGridPuzzle puzzleController;
    private int row, col;
    
    public void Initialize(SleightGridPuzzle controller, int buttonRow, int buttonCol)
    {
        puzzleController = controller;
        row = buttonRow;
        col = buttonCol;
    }
    
    void OnMouseDown()
    {
        if (puzzleController != null)
        {
            puzzleController.HandleButtonClick(row, col);
        }
    }
}
