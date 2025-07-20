using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simplified single-file Power Flow Puzzle
/// Use PuzzleButton3D for interaction
/// </summary>
public class PowerFlowPuzzleSingleFile : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridSize = 5;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private float buttonSpacing = 2f;

    [Header("Visuals")]
    [SerializeField] private Material inactiveMat;
    [SerializeField] private Material activeMat;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip winSound;

    [Header("Events")]
    public UnityEvent OnPuzzleComplete;

    private bool[,] isActive;
    private GameObject[,] buttons;
    private bool puzzleComplete;
    
    // Public property for completion status
    public bool IsPuzzleComplete => puzzleComplete;

    void Start()
    {
        InitGrid();
    }

    private void InitGrid()
    {
        isActive = new bool[gridSize, gridSize];
        buttons = new GameObject[gridSize, gridSize];
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                CreateButton(x, y);
            }
        }
    }

    private void CreateButton(int x, int y)
    {
        GameObject button = Instantiate(buttonPrefab, gridParent);
        button.transform.localPosition = new Vector3(x * buttonSpacing, 0, y * buttonSpacing);
        button.name = $"Button_{x}_{y}";
        buttons[x, y] = button;

        var puzzleButton = button.GetComponent<PuzzleButton3D>();
        if (puzzleButton == null)
            puzzleButton = button.AddComponent<PuzzleButton3D>();

        PowerFlowButtonHandler clickHandler = button.AddComponent<PowerFlowButtonHandler>();
        clickHandler.Setup(this, x, y);

        SetButtonState(x, y, false);
    }

    public void OnButtonClicked(int x, int y)
    {
        if (puzzleComplete) return;

        isActive[x, y] = !isActive[x, y];
        if (audioSource != null) audioSource.PlayOneShot(clickSound);
        UpdateVisuals(x, y);

        if (CheckCompletion())
        {
            puzzleComplete = true;
            if (audioSource != null) audioSource.PlayOneShot(winSound);
            OnPuzzleComplete?.Invoke();
        }
    }

    private bool CheckCompletion()
    {
        // Simplified logic check: complete if all buttons are active
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                if (!isActive[x, y]) return false;

        return true;
    }

    private void UpdateVisuals(int x, int y)
    {
        var renderer = buttons[x, y].GetComponent<Renderer>();
        if (renderer != null)
            renderer.material = isActive[x, y] ? activeMat : inactiveMat;
    }

    private void SetButtonState(int x, int y, bool state)
    {
        isActive[x, y] = state;
        UpdateVisuals(x, y);
    }
}

public class PowerFlowButtonHandler : MonoBehaviour
{
    private PowerFlowPuzzleSingleFile puzzle;
    private int x, y;

    public void Setup(PowerFlowPuzzleSingleFile parentPuzzle, int xCoord, int yCoord)
    {
        puzzle = parentPuzzle;
        x = xCoord;
        y = yCoord;
    }

    void OnMouseDown()
    {
        puzzle.OnButtonClicked(x, y);
    }
}
