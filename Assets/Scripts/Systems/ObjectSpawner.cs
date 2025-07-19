using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] objectsToSpawn; // Objects to be spawned
    [SerializeField] private float chunkSize = 1000f;
    [SerializeField] private float spawnDistance = 500f; // Min distance from player to spawn
    [SerializeField] private float Y = 0f;

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

        foreach (GameObject prefab in objectsToSpawn)
        {
            Vector3 spawnPosition;
            do
            {
                spawnPosition = new Vector3(
                    Random.Range(spawnCenter.x - chunkSize / 2, spawnCenter.x + chunkSize / 2),
                    Y,
                    Random.Range(spawnCenter.z - chunkSize / 2, spawnCenter.z + chunkSize / 2)
                );
            } while (Vector3.Distance(spawnPosition, player.position) < spawnDistance);

            Instantiate(prefab, spawnPosition, Quaternion.identity).isStatic = true;
        }
    }

    private void UpdateCurrentChunk()
    {
        currentChunk = GetPlayerChunk();
    }
}
