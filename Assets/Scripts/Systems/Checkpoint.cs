using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public string checkpointID;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveGameAtCheckpoint(other.transform);
        }
    }

    private void SaveGameAtCheckpoint(Transform playerTransform)
    {
        SaveData saveData = SaveSystem.LoadGame();
        saveData.playerPosition = playerTransform.position;
        saveData.lastCheckpointId = checkpointID;
        saveData.currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Update other saveData fields as needed

        SaveSystem.SaveGame(saveData);
    }
}
