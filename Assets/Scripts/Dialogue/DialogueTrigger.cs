using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public string dialogueScenePath;
    public string triggerId;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!string.IsNullOrEmpty(dialogueScenePath))
        {
            DialogueManager.Instance.LoadAndStartScene(dialogueScenePath);
        }
        if (!string.IsNullOrEmpty(triggerId))
        {
            TriggerManager.Instance.Trigger(triggerId, null);
        }
    }
}

