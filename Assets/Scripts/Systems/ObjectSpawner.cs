using UnityEngine;

[System.Serializable]
public class SpawnableObject
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float frequency = 0.5f; // Probability of spawning this object (0 = never, 1 = always)
    public float yOffset = 0f; // Y coordinate offset for this specific object
    [Range(1, 10)]
    public int maxPerChunk = 1; // Maximum number of this object type per chunk
}

public class ObjectSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private SpawnableObject[] objectsToSpawn; // Objects to be spawned with frequency settings
    [SerializeField] private float chunkSize = 1000f;
    [SerializeField] private float spawnDistance = 500f; // Min distance from player to spawn
    [SerializeField] private float baseY = 0f; // Base Y coordinate for all objects

    private Vector2 currentChunk;

    void Start()
    {
        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("ObjectSpawner: No player reference assigned and no GameObject with 'Player' tag found!");
        }

        UpdateCurrentChunk();
    }

    void Update()
    {
        if (player == null) return;

        Vector2 newChunk = GetPlayerChunk();
        if (newChunk != currentChunk)
        {
            currentChunk = newChunk;
            SpawnObjects();
        }
    }

    private Vector2 GetPlayerChunk()
    {
        return new Vector2(
            Mathf.Round(player.position.x / chunkSize),
            Mathf.Round(player.position.z / chunkSize)
        );
    }

    private void SpawnObjects()
    {
        Vector3 spawnCenter = new Vector3(currentChunk.x * chunkSize, 0, currentChunk.y * chunkSize);

        foreach (SpawnableObject spawnableObj in objectsToSpawn)
        {
            // Check if we should spawn this object based on its frequency
            if (Random.Range(0f, 1f) > spawnableObj.frequency)
                continue;

            Vector3 spawnPosition;
            do
            {
                spawnPosition = new Vector3(
                    Random.Range(spawnCenter.x - chunkSize / 2, spawnCenter.x + chunkSize / 2),
                    baseY + spawnableObj.yOffset,
                    Random.Range(spawnCenter.z - chunkSize / 2, spawnCenter.z + chunkSize / 2)
                );
            } while (Vector3.Distance(spawnPosition, player.position) < spawnDistance);

            Instantiate(spawnableObj.prefab, spawnPosition, Quaternion.identity).isStatic = true;
        }
    }

    private void UpdateCurrentChunk()
    {
        currentChunk = GetPlayerChunk();
    }
}
