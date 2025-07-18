using UnityEngine;

// TriggerManager works with DialogueManager
public class TriggerManager : MonoBehaviour
{
    public static TriggerManager Instance;
    private void Awake() => Instance = this;

    public void Trigger(string triggerName, DialogueLine line)
    {
        switch (triggerName)
        {
            case "start_quest":
                // Quest system removed - placeholder for future implementation
                Debug.Log("Quest system not implemented yet");
                break;
            case "scene_transition":
                if(line != null)
                    DialogueManager.Instance.LoadScene(line.next_scene);
                break;
            default:
                Debug.LogWarning("Unknown trigger: " + triggerName);
                break;
        }
    }
}

