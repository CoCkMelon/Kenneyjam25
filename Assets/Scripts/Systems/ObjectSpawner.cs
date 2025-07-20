using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawnableObject
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float frequency = 0.5f; // Probability of spawning this object (0 = never, 1 = always)
    public float yOffset = 0f; // Y coordinate offset for this specific object
    [Range(1, 100)]
    public int minPerChunk = 1; // Minimum number of this object type per chunk
    [Range(1, 1000)]
    public int maxPerChunk = 10; // Maximum number of this object type per chunk
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
    [SerializeField] private int spawnRadius = 2; // Number of chunks around player to spawn objects
    [SerializeField] private bool enableMassiveSpawning = true; // Toggle for massive spawning mode
    [SerializeField] private int maxTotalObjectsPerFrame = 100; // Limit objects spawned per frame for performance

    private Vector2 currentChunk;
    private HashSet<Vector2> spawnedChunks = new HashSet<Vector2>(); // Track which chunks have been spawned
    private Transform spawnedObjectsParent; // Parent object to organize spawned objects

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

        // Create parent object for spawned objects organization
        GameObject parentObj = new GameObject("Spawned Objects");
        spawnedObjectsParent = parentObj.transform;

        UpdateCurrentChunk();
        
        // Initial spawn around player position
        if (enableMassiveSpawning)
        {
            SpawnAroundPlayer();
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector2 newChunk = GetPlayerChunk();
        if (newChunk != currentChunk)
        {
            currentChunk = newChunk;
            
            if (enableMassiveSpawning)
            {
                SpawnAroundPlayer();
            }
            else
            {
                SpawnObjects();
            }
        }
    }

    private Vector2 GetPlayerChunk()
    {
        return new Vector2(
            Mathf.Round(player.position.x / chunkSize),
            Mathf.Round(player.position.z / chunkSize)
        );
    }

    private void SpawnAroundPlayer()
    {
        int totalObjectsSpawned = 0;
        
        // Spawn objects in a radius around the player's current chunk
        for (int x = -spawnRadius; x <= spawnRadius; x++)
        {
            for (int z = -spawnRadius; z <= spawnRadius; z++)
            {
                Vector2 chunkToSpawn = new Vector2(currentChunk.x + x, currentChunk.y + z);
                
                // Skip if this chunk has already been spawned
                if (spawnedChunks.Contains(chunkToSpawn))
                    continue;
                
                // Mark this chunk as spawned
                spawnedChunks.Add(chunkToSpawn);
                
                // Spawn objects in this chunk
                totalObjectsSpawned += SpawnObjectsInChunk(chunkToSpawn);
                
                // Break if we've hit our per-frame limit
                if (totalObjectsSpawned >= maxTotalObjectsPerFrame)
                    return;
            }
        }
    }
    
    private int SpawnObjectsInChunk(Vector2 chunkCoords)
    {
        Vector3 spawnCenter = new Vector3(chunkCoords.x * chunkSize, 0, chunkCoords.y * chunkSize);
        int objectsSpawnedInChunk = 0;

        foreach (SpawnableObject spawnableObj in objectsToSpawn)
        {
            // Check if we should spawn this object based on its frequency
            if (Random.Range(0f, 1f) > spawnableObj.frequency)
                continue;

            // Determine how many objects to spawn for this type
            int objectsToSpawnCount = Random.Range(spawnableObj.minPerChunk, spawnableObj.maxPerChunk + 1);

            // Spawn the determined number of objects
            for (int i = 0; i < objectsToSpawnCount; i++)
            {
                Vector3 spawnPosition = new Vector3(
                    Random.Range(spawnCenter.x - chunkSize / 2, spawnCenter.x + chunkSize / 2),
                    baseY + spawnableObj.yOffset,
                    Random.Range(spawnCenter.z - chunkSize / 2, spawnCenter.z + chunkSize / 2)
                );
                
                // For massive spawning, we don't enforce minimum distance from player to allow dense spawning
                GameObject spawnedObject = Instantiate(spawnableObj.prefab, spawnPosition, Quaternion.identity, spawnedObjectsParent);
                spawnedObject.isStatic = true;
                
                // Optional: Add some random rotation for variety
                spawnedObject.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                
                objectsSpawnedInChunk++;
            }
        }
        
        return objectsSpawnedInChunk;
    }

    private void SpawnObjects()
    {
        Vector3 spawnCenter = new Vector3(currentChunk.x * chunkSize, 0, currentChunk.y * chunkSize);

        foreach (SpawnableObject spawnableObj in objectsToSpawn)
        {
            // Check if we should spawn this object based on its frequency
            if (Random.Range(0f, 1f) > spawnableObj.frequency)
                continue;

            // Determine how many objects to spawn for this type
            int objectsToSpawnCount = Random.Range(spawnableObj.minPerChunk, spawnableObj.maxPerChunk + 1);

            // Spawn the determined number of objects
            for (int i = 0; i < objectsToSpawnCount; i++)
            {
                Vector3 spawnPosition;
                int attemptCount = 0;
                const int maxAttempts = 50; // Prevent infinite loops

                do
                {
                    spawnPosition = new Vector3(
                        Random.Range(spawnCenter.x - chunkSize / 2, spawnCenter.x + chunkSize / 2),
                        baseY + spawnableObj.yOffset,
                        Random.Range(spawnCenter.z - chunkSize / 2, spawnCenter.z + chunkSize / 2)
                    );
                    attemptCount++;
                } while (Vector3.Distance(spawnPosition, player.position) < spawnDistance && attemptCount < maxAttempts);

                // Only spawn if we found a valid position or reached max attempts
                if (attemptCount < maxAttempts || Vector3.Distance(spawnPosition, player.position) >= spawnDistance)
                {
                    GameObject spawnedObject = Instantiate(spawnableObj.prefab, spawnPosition, Quaternion.identity, spawnedObjectsParent);
                    spawnedObject.isStatic = true;
                    
                    // Optional: Add some random rotation for variety
                    spawnedObject.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                }
            }
        }
    }

    private void UpdateCurrentChunk()
    {
        currentChunk = GetPlayerChunk();
    }
}
