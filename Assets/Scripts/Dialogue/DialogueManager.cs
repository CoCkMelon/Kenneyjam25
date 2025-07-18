using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public string currentScenePath;
    private DialogueScene sceneData;
    private int currentIndex = 0;
    private Dictionary<string, int> labelledLines = new Dictionary<string, int>();
    private DialogueUIController uiController;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        uiController = DialogueUIController.Instance;
        if (uiController == null)
        {
            Debug.LogError("DialogueUIController not found! Please add it to the scene.");
        }
    }

    public void LoadAndStartScene(string path)
    {
        sceneData = DialogueLoaderEnhanced.Load(path);
        if (sceneData == null)
        {
            Debug.LogError($"Failed to load dialogue scene: {path}");
            return;
        }
        currentIndex = 0;
        IndexLabelledLines();
        PlayCurrentLine();
    }
    
    public void LoadAndStartSceneFromResources(string resourcePath)
    {
        sceneData = DialogueLoaderEnhanced.LoadFromResources(resourcePath);
        if (sceneData == null)
        {
            Debug.LogError($"Failed to load dialogue scene from Resources: {resourcePath}");
            return;
        }
        currentIndex = 0;
        IndexLabelledLines();
        PlayCurrentLine();
    }
    

    void IndexLabelledLines() {
        if (sceneData?.lines != null) {
            for (int i = 0; i < sceneData.lines.Count; i++) {
                if (!string.IsNullOrEmpty(sceneData.lines[i].id)) {
                    labelledLines[sceneData.lines[i].id] = i;
                }
            }
        }
    }

    void PlayCurrentLine()
    {
        if (sceneData == null || sceneData.lines == null || currentIndex >= sceneData.lines.Count)
        {
            Debug.Log("Scene finished!");
            if (uiController != null)
                uiController.HideDialogue();
            return;
        }
        var line = sceneData.lines[currentIndex];
        
        // Check if this line has choices
        if (line.options != null && line.options.Count > 0)
        {
            DisplayOptions(line.options);
        }
        
        // Use the new UI system
        if (uiController != null)
        {
            uiController.ShowDialogue(line);
        }
        else
        {
            // Fallback to debug logs
            if (!string.IsNullOrEmpty(line.speaker))
                Debug.Log($"{line.speaker}: {line.text}");
            else if (!string.IsNullOrEmpty(line.text))
                Debug.Log(line.text);
        }
        
        if (!string.IsNullOrEmpty(line.trigger))
            TriggerManager.Instance.Trigger(line.trigger, line);
    }

    public void AdvanceDialogue()
    {
        currentIndex++;
        if (currentIndex < sceneData.lines.Count)
        {
            PlayCurrentLine();
        }
        else
        {
            // End of dialogue
            if (uiController != null)
                uiController.HideDialogue();
        }
    }

    // WARNING: Choice handling must be implemented in C# code!
    // This method shows choices in UI but doesn't handle the logic
    private void DisplayOptions(List<DialogueOption> options) {
        Debug.LogWarning("CHOICES DETECTED: Choice logic must be implemented in C# code!");
        Debug.LogWarning("Available options:");
        foreach (var option in options) {
            Debug.LogWarning($"  - {option.choice} -> {option.next}");
        }
        Debug.LogWarning("Implement choice handling in your game logic!");
    }

    public void LoadScene(string nextScene)
    {
        // Logic to switch/load scenes
        Debug.Log($"Switch to scene: {nextScene}");
    }

    public void SelectChoice(string nextLineId)
    {
        if (labelledLines.TryGetValue(nextLineId, out int lineIndex))
        {
            currentIndex = lineIndex;
            PlayCurrentLine();
        }
        else
        {
            Debug.LogWarning($"Line with ID '{nextLineId}' not found!");
            AdvanceDialogue();
        }
    }
    
    public DialogueLine GetCurrentLine()
    {
        if (sceneData != null && sceneData.lines != null && currentIndex >= 0 && currentIndex < sceneData.lines.Count)
        {
            return sceneData.lines[currentIndex];
        }
        return null;
    }
}

