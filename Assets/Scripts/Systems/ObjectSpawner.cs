using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawnableObject
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float frequency = 0.5f; // Probability of spawning this object (0 = never, 1 = always)
    public float yOffset = 0f; // Y coordinate offset for this specific object
    [Range(0, 100)]
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
    [SerializeField] private int forwardSpawnDistance = 3; // How many chunks ahead to spawn
    [SerializeField] private int sideSpawnRadius = 1; // How many chunks to the sides to spawn
    [SerializeField] private bool enableMassiveSpawning = true; // Toggle for massive spawning mode
    [SerializeField] private int maxTotalObjectsPerFrame = 100; // Limit objects spawned per frame for performance
    [SerializeField] private bool spawnAheadOfMovement = true; // Spawn objects ahead of player movement
    [SerializeField] private float cleanupDistance = 2000f; // Distance behind player to cleanup objects
    [SerializeField] private bool enableCleanup = true; // Enable cleanup of objects behind player

    private Vector2 currentChunk;
    private Vector3 lastPlayerPosition;
    private Vector2 movementDirection;
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

        // Initialize movement tracking
        lastPlayerPosition = player.position;
        movementDirection = Vector2.zero;

        UpdateCurrentChunk();
        
        // Initial spawn around player position
        if (enableMassiveSpawning)
        {
            if (spawnAheadOfMovement)
                SpawnAheadOfPlayer();
            else
                SpawnAroundPlayer();
        }
    }

    void Update()
    {
        if (player == null) return;

        // Update movement direction
        Vector3 currentPosition = player.position;
        Vector3 deltaMovement = currentPosition - lastPlayerPosition;
        
        // Only update direction if player is actually moving (avoid jitter)
        if (deltaMovement.magnitude > 0.1f)
        {
            movementDirection = new Vector2(deltaMovement.x, deltaMovement.z).normalized;
        }
        lastPlayerPosition = currentPosition;

        Vector2 newChunk = GetPlayerChunk();
        if (newChunk != currentChunk)
        {
            currentChunk = newChunk;
            
            if (enableMassiveSpawning)
            {
                if (spawnAheadOfMovement && movementDirection != Vector2.zero)
                    SpawnAheadOfPlayer();
                else
                    SpawnAroundPlayer();
            }
            else
            {
                SpawnObjects();
            }
        }
        
        // Periodically cleanup objects behind player
        if (enableCleanup && Time.frameCount % 60 == 0) // Every 60 frames (roughly once per second)
        {
            CleanupObjectsBehindPlayer();
        }
    }

    private Vector2 GetPlayerChunk()
    {
        return new Vector2(
            Mathf.Round(player.position.x / chunkSize),
            Mathf.Round(player.position.z / chunkSize)
        );
    }

    private void SpawnAheadOfPlayer()
    {
        int totalObjectsSpawned = 0;
        
        // Calculate forward direction in chunk coordinates
        Vector2 forwardDir = movementDirection.normalized;
        
        // Spawn chunks ahead of the player
        for (int forward = 1; forward <= forwardSpawnDistance; forward++)
        {
            for (int side = -sideSpawnRadius; side <= sideSpawnRadius; side++)
            {
                // Calculate chunk position ahead of player movement
                Vector2 forwardOffset = forwardDir * forward;
                Vector2 sideOffset = new Vector2(-forwardDir.y, forwardDir.x) * side; // Perpendicular to movement
                Vector2 chunkToSpawn = currentChunk + forwardOffset + sideOffset;
                
                // Round to ensure we're dealing with whole chunks
                chunkToSpawn = new Vector2(Mathf.Round(chunkToSpawn.x), Mathf.Round(chunkToSpawn.y));
                
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
        
        // Also spawn in the current chunk and immediate sides if not already spawned
        for (int side = -sideSpawnRadius; side <= sideSpawnRadius; side++)
        {
            Vector2 sideOffset = new Vector2(-movementDirection.y, movementDirection.x) * side;
            Vector2 chunkToSpawn = currentChunk + sideOffset;
            chunkToSpawn = new Vector2(Mathf.Round(chunkToSpawn.x), Mathf.Round(chunkToSpawn.y));
            
            if (!spawnedChunks.Contains(chunkToSpawn))
            {
                spawnedChunks.Add(chunkToSpawn);
                totalObjectsSpawned += SpawnObjectsInChunk(chunkToSpawn);
                
                if (totalObjectsSpawned >= maxTotalObjectsPerFrame)
                    return;
            }
        }
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
    
    private void CleanupObjectsBehindPlayer()
    {
        if (spawnedObjectsParent == null || movementDirection == Vector2.zero)
            return;
            
        Vector3 playerPos = player.position;
        Vector3 behindDirection = new Vector3(-movementDirection.x, 0, -movementDirection.y);
        
        // Get all spawned objects
        Transform[] children = new Transform[spawnedObjectsParent.childCount];
        for (int i = 0; i < spawnedObjectsParent.childCount; i++)
        {
            children[i] = spawnedObjectsParent.GetChild(i);
        }
        
        // Check each object and destroy if too far behind
        foreach (Transform child in children)
        {
            if (child == null) continue;
            
            Vector3 toObject = child.position - playerPos;
            float behindDistance = Vector3.Dot(toObject, behindDirection);
            
            // If object is behind the player and beyond cleanup distance, destroy it
            if (behindDistance > 0 && Vector3.Distance(child.position, playerPos) > cleanupDistance)
            {
                // Also remove the chunk from spawned chunks so it can be respawned later
                Vector2 objectChunk = new Vector2(
                    Mathf.Round(child.position.x / chunkSize),
                    Mathf.Round(child.position.z / chunkSize)
                );
                spawnedChunks.Remove(objectChunk);
                
                DestroyImmediate(child.gameObject);
            }
        }
    }
}
