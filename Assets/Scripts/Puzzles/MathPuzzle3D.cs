using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// 3D Math Puzzle with 3D symbols and number buttons
/// Players solve simple math equations by clicking 3D number buttons
/// </summary>
public class MathPuzzle3D : MonoBehaviour
{
    [System.Serializable]
    public class MathEquation
    {
        public int operand1;
        public MathOperation operation;
        public int operand2;
        public int answer => CalculateAnswer();
        
        public string GetDisplayString()
        {
            return $"{operand1} {GetOperationSymbol()} {operand2} = ?";
        }
        
        private int CalculateAnswer()
        {
            switch (operation)
            {
                case MathOperation.Add: return operand1 + operand2;
                case MathOperation.Subtract: return operand1 - operand2;
                case MathOperation.Multiply: return operand1 * operand2;
                case MathOperation.Divide: return operand2 != 0 ? operand1 / operand2 : 0;
                default: return 0;
            }
        }
        
        private string GetOperationSymbol()
        {
            switch (operation)
            {
                case MathOperation.Add: return "+";
                case MathOperation.Subtract: return "-";
                case MathOperation.Multiply: return "×";
                case MathOperation.Divide: return "÷";
                default: return "?";
            }
        }
    }
    
    public enum MathOperation
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }
    
    [Header("Puzzle Configuration")]
    [SerializeField] private List<MathEquation> equations = new List<MathEquation>();
    [SerializeField] private int currentEquationIndex = 0;
    [SerializeField] private bool randomizeEquations = true;
    [SerializeField] private int maxEquations = 5;
    
    [Header("3D Symbol Elements")]
    [SerializeField] private Transform equationDisplay;
    [SerializeField] private Transform numberButtonsContainer;
    [SerializeField] private GameObject numberButtonPrefab;
    [SerializeField] private Transform answerDisplay;
    
    [Header("3D Symbol Prefabs")]
    [SerializeField] private GameObject[] numberSymbolPrefabs = new GameObject[10]; // 0-9
    [SerializeField] private GameObject plusSymbolPrefab;
    [SerializeField] private GameObject minusSymbolPrefab;
    [SerializeField] private GameObject multiplySymbolPrefab;
    [SerializeField] private GameObject divideSymbolPrefab;
    [SerializeField] private GameObject equalsSymbolPrefab;
    [SerializeField] private GameObject questionMarkPrefab;
    
    [Header("Layout Settings")]
    [SerializeField] private Vector3 buttonSpacing = new Vector3(1.5f, 0f, 0f);
    [SerializeField] private int buttonsPerRow = 5;
    [SerializeField] private float rowSpacing = 1.5f;
    
    [Header("Positioning & Rotation Control")]
    [SerializeField] private Vector3 equationPosition = Vector3.zero;
    [SerializeField] private Vector3 equationRotation = Vector3.zero;
    [SerializeField] private Vector3 equationScale = Vector3.one;
    [SerializeField] private Vector3 answerPosition = new Vector3(0f, -2f, 0f);
    [SerializeField] private Vector3 answerRotation = Vector3.zero;
    [SerializeField] private Vector3 answerScale = Vector3.one;
    [SerializeField] private Vector3 buttonsPosition = new Vector3(0f, -4f, 0f);
    [SerializeField] private Vector3 buttonsRotation = Vector3.zero;
    [SerializeField] private Vector3 buttonsScale = Vector3.one;
    
    [Header("Symbol Spacing & Positioning")]
    [SerializeField] private float symbolSpacing = 1.2f;
    [SerializeField] private float digitSpacing = 0.6f;
    [SerializeField] private float equationCenterOffset = -2.4f;
    private void ApplyPuzzleTransforms()
    {
        if (equationDisplay != null)
        {
            equationDisplay.localPosition = equationPosition;
            equationDisplay.localRotation = Quaternion.Euler(equationRotation);
            equationDisplay.localScale = equationScale;
        }

        if (answerDisplay != null)
        {
            answerDisplay.localPosition = answerPosition;
            answerDisplay.localRotation = Quaternion.Euler(answerRotation);
            answerDisplay.localScale = answerScale;
        }

        if (numberButtonsContainer != null)
        {
            numberButtonsContainer.localPosition = buttonsPosition;
            numberButtonsContainer.localRotation = Quaternion.Euler(buttonsRotation);
            numberButtonsContainer.localScale = buttonsScale;
        }
    }
    
    [Header("Feedback")]
    [SerializeField] private GameObject correctFeedback;
    [SerializeField] private GameObject incorrectFeedback;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color incorrectColor = Color.red;
    
    [Header("Animation")]
    [SerializeField] private float feedbackDuration = 2f;
    [SerializeField] private float nextEquationDelay = 1f;

    // State
    private string currentPlayerAnswer = "";
    private List<MathButton3D> numberButtons = new List<MathButton3D>();
    private bool isPuzzleComplete = false;
    private int correctAnswersCount = 0;
    private AudioSource audioSource;
    
    // 3D Symbol Display
    private List<GameObject> currentEquationSymbols = new List<GameObject>();
    private List<GameObject> currentAnswerSymbols = new List<GameObject>();

    // Events
    [Header("Events")]
    public UnityEvent OnPuzzleComplete;
    public UnityEvent<int> OnCorrectAnswer;
    public UnityEvent<int> OnIncorrectAnswer;

    void Start()
    {
        InitializeAudio();
        SetupEquations();
        ApplyPuzzleTransforms();
        CreateNumberButtons();
        DisplayCurrentEquation();
    }

    private void InitializeAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void SetupEquations()
    {
        if (equations.Count == 0)
        {
            GenerateRandomEquations();
        }

        if (randomizeEquations)
        {
            ShuffleEquations();
        }

        // Ensure we don't exceed max equations
        if (equations.Count > maxEquations)
        {
            equations.RemoveRange(maxEquations, equations.Count - maxEquations);
        }
    }

    private void GenerateRandomEquations()
    {
        equations.Clear();

        for (int i = 0; i < maxEquations; i++)
        {
            MathEquation equation = new MathEquation();

            // Generate based on difficulty progression
            int difficulty = Mathf.Min(i / 2, 3); // Increase difficulty every 2 equations

            switch (difficulty)
            {
                case 0: // Easy addition/subtraction
                    equation.operation = Random.Range(0, 2) == 0 ? MathOperation.Add : MathOperation.Subtract;
                    equation.operand1 = Random.Range(1, 10);
                    equation.operand2 = Random.Range(1, 10);
                    if (equation.operation == MathOperation.Subtract && equation.operand2 > equation.operand1)
                    {
                        // Swap to avoid negative results
                        int temp = equation.operand1;
                        equation.operand1 = equation.operand2;
                        equation.operand2 = temp;
                    }
                    break;

                case 1: // Medium addition/subtraction with larger numbers
                    equation.operation = Random.Range(0, 2) == 0 ? MathOperation.Add : MathOperation.Subtract;
                    equation.operand1 = Random.Range(10, 50);
                    equation.operand2 = Random.Range(1, 20);
                    if (equation.operation == MathOperation.Subtract && equation.operand2 > equation.operand1)
                    {
                        int temp = equation.operand1;
                        equation.operand1 = equation.operand2;
                        equation.operand2 = temp;
                    }
                    break;

                case 2: // Multiplication
                    equation.operation = MathOperation.Multiply;
                    equation.operand1 = Random.Range(2, 10);
                    equation.operand2 = Random.Range(2, 10);
                    break;

                case 3: // Division (ensure clean division)
                    equation.operation = MathOperation.Divide;
                    equation.operand2 = Random.Range(2, 10);
                    int multiplier = Random.Range(2, 10);
                    equation.operand1 = equation.operand2 * multiplier;
                    break;
            }

            equations.Add(equation);
        }
    }

    private void ShuffleEquations()
    {
        for (int i = 0; i < equations.Count; i++)
        {
            int randomIndex = Random.Range(i, equations.Count);
            MathEquation temp = equations[i];
            equations[i] = equations[randomIndex];
            equations[randomIndex] = temp;
        }
    }

    private void CreateNumberButtons()
    {
        if (numberButtonPrefab == null || numberButtonsContainer == null)
        {
            Debug.LogError("Number button prefab or container not assigned!");
            return;
        }

        // Create buttons 0-9
        for (int i = 0; i <= 9; i++)
        {
            CreateNumberButton(i);
        }

        // Add special buttons
        CreateClearButton();
        CreateSubmitButton();
    }

    private void CreateNumberButton(int number)
    {
        GameObject buttonObj = Instantiate(numberButtonPrefab, numberButtonsContainer);

        // Position button in grid
        int row = number / buttonsPerRow;
        int col = number % buttonsPerRow;

        Vector3 position = new Vector3(
            col * buttonSpacing.x,
            -row * rowSpacing,
            0f
        );

        buttonObj.transform.localPosition = position;

        // Set up button component
        MathButton3D button = buttonObj.GetComponent<MathButton3D>();
        if (button == null)
        {
            button = buttonObj.AddComponent<MathButton3D>();
        }

        GameObject symbolPrefab = numberSymbolPrefabs[number];
        button.Initialize(number.ToString(), symbolPrefab, () => OnNumberButtonPressed(number));
        numberButtons.Add(button);
    }

    private void CreateClearButton()
    {
        GameObject buttonObj = Instantiate(numberButtonPrefab, numberButtonsContainer);

        // Position at end of grid
        Vector3 position = new Vector3(
            0f * buttonSpacing.x,
            -2f * rowSpacing,
            0f
        );

        buttonObj.transform.localPosition = position;

        MathButton3D button = buttonObj.GetComponent<MathButton3D>();
        if (button == null)
        {
            button = buttonObj.AddComponent<MathButton3D>();
        }

        button.Initialize("Clear", ClearAnswer);
        numberButtons.Add(button);
    }

    private void CreateSubmitButton()
    {
        GameObject buttonObj = Instantiate(numberButtonPrefab, numberButtonsContainer);

        // Position at end of grid
        Vector3 position = new Vector3(
            2f * buttonSpacing.x,
            -2f * rowSpacing,
            0f
        );

        buttonObj.transform.localPosition = position;

        MathButton3D button = buttonObj.GetComponent<MathButton3D>();
        if (button == null)
        {
            button = buttonObj.AddComponent<MathButton3D>();
        }

        button.Initialize("Submit", SubmitAnswer);
        numberButtons.Add(button);
    }

    private void OnNumberButtonPressed(int number)
    {
        if (isPuzzleComplete) return;

        // Play button click sound
        if (audioSource && buttonClickSound)
            audioSource.PlayOneShot(buttonClickSound);

        // Add number to current answer (limit to reasonable length)
        if (currentPlayerAnswer.Length < 5)
        {
            currentPlayerAnswer += number.ToString();
            UpdateAnswerDisplay();
        }
    }

    private void ClearAnswer()
    {
        if (isPuzzleComplete) return;

        currentPlayerAnswer = "";
        UpdateAnswerDisplay();

        if (audioSource && buttonClickSound)
            audioSource.PlayOneShot(buttonClickSound);
    }

    private void SubmitAnswer()
    {
        if (isPuzzleComplete || string.IsNullOrEmpty(currentPlayerAnswer)) return;

        if (int.TryParse(currentPlayerAnswer, out int playerAnswer))
        {
            CheckAnswer(playerAnswer);
        }
    }

    private void CheckAnswer(int playerAnswer)
    {
        MathEquation currentEquation = equations[currentEquationIndex];
        bool isCorrect = playerAnswer == currentEquation.answer;

        StartCoroutine(ShowAnswerFeedback(isCorrect));

        if (isCorrect)
        {
            correctAnswersCount++;
            OnCorrectAnswer?.Invoke(currentEquationIndex);

            if (audioSource && correctSound)
                audioSource.PlayOneShot(correctSound);
        }
        else
        {
            OnIncorrectAnswer?.Invoke(currentEquationIndex);

            if (audioSource && incorrectSound)
                audioSource.PlayOneShot(incorrectSound);
        }
    }

    private IEnumerator ShowAnswerFeedback(bool isCorrect)
    {
        // Show feedback
        GameObject feedbackObj = isCorrect ? correctFeedback : incorrectFeedback;
        if (feedbackObj != null)
        {
            feedbackObj.SetActive(true);
        }

        // Change answer symbols color
        ChangeAnswerSymbolsColor(isCorrect ? correctColor : incorrectColor);
        
        yield return new WaitForSeconds(feedbackDuration);
        
        // Hide feedback
        if (feedbackObj != null)
        {
            feedbackObj.SetActive(false);
        }
        
        // Reset answer symbols color
        ChangeAnswerSymbolsColor(Color.white);

        yield return new WaitForSeconds(nextEquationDelay);

        if (isCorrect)
        {
            NextEquation();
        }
        else
        {
            // Allow retry
            ClearAnswer();
        }
    }

    private void NextEquation()
    {
        currentEquationIndex++;

        if (currentEquationIndex >= equations.Count)
        {
            CompletePuzzle();
        }
        else
        {
            ClearAnswer();
            DisplayCurrentEquation();
        }
    }

    private void CompletePuzzle()
    {
        isPuzzleComplete = true;
        OnPuzzleComplete?.Invoke();

        Debug.Log($"Math puzzle completed! Correct answers: {correctAnswersCount}/{equations.Count}");
    }

    private void DisplayCurrentEquation()
    {
        if (currentEquationIndex >= equations.Count) return;
        
        // Clear existing equation symbols
        ClearEquationSymbols();
        
        MathEquation equation = equations[currentEquationIndex];
        
        // Create 3D symbols for the equation
        CreateEquationSymbols(equation);
    }
    
    private void UpdateAnswerDisplay()
    {
        // Clear existing answer symbols
        ClearAnswerSymbols();
        
        if (string.IsNullOrEmpty(currentPlayerAnswer))
        {
            // Show question mark
            if (questionMarkPrefab != null && answerDisplay != null)
            {
                GameObject questionMark = Instantiate(questionMarkPrefab, answerDisplay);
                questionMark.transform.localPosition = Vector3.zero;
                currentAnswerSymbols.Add(questionMark);
            }
        }
        else
        {
            // Create 3D number symbols for the answer
            CreateAnswerSymbols(currentPlayerAnswer);
        }
    }
    
    private void CreateEquationSymbols(MathEquation equation)
    {
        if (equationDisplay == null) return;
        
        float startX = equationCenterOffset; // Configurable center offset
        
        // First operand
        CreateNumberSymbols(equation.operand1, equationDisplay, new Vector3(startX, 0, 0));
        
        // Operation symbol
        GameObject operationSymbol = GetOperationSymbolPrefab(equation.operation);
        if (operationSymbol != null)
        {
            GameObject opInstance = Instantiate(operationSymbol, equationDisplay);
            opInstance.transform.localPosition = new Vector3(startX + symbolSpacing, 0, 0);
            currentEquationSymbols.Add(opInstance);
        }
        
        // Second operand
        CreateNumberSymbols(equation.operand2, equationDisplay, new Vector3(startX + symbolSpacing * 2, 0, 0));
        
        // Equals symbol
        if (equalsSymbolPrefab != null)
        {
            GameObject equalsInstance = Instantiate(equalsSymbolPrefab, equationDisplay);
            equalsInstance.transform.localPosition = new Vector3(startX + symbolSpacing * 3, 0, 0);
            currentEquationSymbols.Add(equalsInstance);
        }
        
        // Question mark
        if (questionMarkPrefab != null)
        {
            GameObject questionInstance = Instantiate(questionMarkPrefab, equationDisplay);
            questionInstance.transform.localPosition = new Vector3(startX + symbolSpacing * 4, 0, 0);
            currentEquationSymbols.Add(questionInstance);
        }
    }
    
    private void CreateNumberSymbols(int number, Transform parent, Vector3 basePosition)
    {
        string numberString = number.ToString();
        
        for (int i = 0; i < numberString.Length; i++)
        {
            int digit = int.Parse(numberString[i].ToString());
            
            if (digit >= 0 && digit <= 9 && numberSymbolPrefabs[digit] != null)
            {
                GameObject digitInstance = Instantiate(numberSymbolPrefabs[digit], parent);
                Vector3 position = basePosition + new Vector3(i * digitSpacing, 0, 0);
                digitInstance.transform.localPosition = position;
                currentEquationSymbols.Add(digitInstance);
            }
        }
    }
    
    private void CreateAnswerSymbols(string answer)
    {
        if (answerDisplay == null) return;
        
        float startX = -(answer.Length - 1) * digitSpacing * 0.5f; // Center the answer
        
        for (int i = 0; i < answer.Length; i++)
        {
            int digit = int.Parse(answer[i].ToString());
            
            if (digit >= 0 && digit <= 9 && numberSymbolPrefabs[digit] != null)
            {
                GameObject digitInstance = Instantiate(numberSymbolPrefabs[digit], answerDisplay);
                digitInstance.transform.localPosition = new Vector3(startX + i * digitSpacing, 0, 0);
                currentAnswerSymbols.Add(digitInstance);
            }
        }
    }
    
    private GameObject GetOperationSymbolPrefab(MathOperation operation)
    {
        switch (operation)
        {
            case MathOperation.Add: return plusSymbolPrefab;
            case MathOperation.Subtract: return minusSymbolPrefab;
            case MathOperation.Multiply: return multiplySymbolPrefab;
            case MathOperation.Divide: return divideSymbolPrefab;
            default: return null;
        }
    }
    
    private void ClearEquationSymbols()
    {
        foreach (GameObject symbol in currentEquationSymbols)
        {
            if (symbol != null)
                Destroy(symbol);
        }
        currentEquationSymbols.Clear();
    }
    
    private void ChangeAnswerSymbolsColor(Color color)
    {
        foreach (GameObject symbol in currentAnswerSymbols)
        {
            Renderer renderer = symbol.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
    }
    
    private void ClearAnswerSymbols()
    {
        foreach (GameObject symbol in currentAnswerSymbols)
        {
            if (symbol != null)
                Destroy(symbol);
        }
        currentAnswerSymbols.Clear();
    }

    public void ResetPuzzle()
    {
        currentEquationIndex = 0;
        correctAnswersCount = 0;
        isPuzzleComplete = false;
        currentPlayerAnswer = "";

        if (randomizeEquations)
        {
            ShuffleEquations();
        }

        DisplayCurrentEquation();
        UpdateAnswerDisplay();

        // Hide feedback objects
        if (correctFeedback != null) correctFeedback.SetActive(false);
        if (incorrectFeedback != null) incorrectFeedback.SetActive(false);
    }

    // Public methods for runtime position/rotation control
    public void SetEquationTransform(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        equationPosition = position;
        equationRotation = rotation;
        equationScale = scale;
        ApplyEquationTransform();
    }
    
    public void SetAnswerTransform(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        answerPosition = position;
        answerRotation = rotation;
        answerScale = scale;
        ApplyAnswerTransform();
    }
    
    public void SetButtonsTransform(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        buttonsPosition = position;
        buttonsRotation = rotation;
        buttonsScale = scale;
        ApplyButtonsTransform();
    }
    
    private void ApplyEquationTransform()
    {
        if (equationDisplay != null)
        {
            equationDisplay.localPosition = equationPosition;
            equationDisplay.localRotation = Quaternion.Euler(equationRotation);
            equationDisplay.localScale = equationScale;
        }
    }
    
    private void ApplyAnswerTransform()
    {
        if (answerDisplay != null)
        {
            answerDisplay.localPosition = answerPosition;
            answerDisplay.localRotation = Quaternion.Euler(answerRotation);
            answerDisplay.localScale = answerScale;
        }
    }
    
    private void ApplyButtonsTransform()
    {
        if (numberButtonsContainer != null)
        {
            numberButtonsContainer.localPosition = buttonsPosition;
            numberButtonsContainer.localRotation = Quaternion.Euler(buttonsRotation);
            numberButtonsContainer.localScale = buttonsScale;
        }
    }
    
    public void SetSymbolSpacing(float newSymbolSpacing, float newDigitSpacing)
    {
        symbolSpacing = newSymbolSpacing;
        digitSpacing = newDigitSpacing;
        // Refresh the current equation display with new spacing
        DisplayCurrentEquation();
        UpdateAnswerDisplay();
    }
    
    // Public properties for external access
    public bool IsPuzzleComplete => isPuzzleComplete;
    public int CurrentEquationIndex => currentEquationIndex;
    public int TotalEquations => equations.Count;
    public int CorrectAnswersCount => correctAnswersCount;
    public string CurrentPlayerAnswer => currentPlayerAnswer;
    
    // Transform getters
    public Vector3 EquationPosition => equationPosition;
    public Vector3 EquationRotation => equationRotation;
    public Vector3 EquationScale => equationScale;
    public Vector3 AnswerPosition => answerPosition;
    public Vector3 AnswerRotation => answerRotation;
    public Vector3 AnswerScale => answerScale;
    public Vector3 ButtonsPosition => buttonsPosition;
    public Vector3 ButtonsRotation => buttonsRotation;
    public Vector3 ButtonsScale => buttonsScale;
}
