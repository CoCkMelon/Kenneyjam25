using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns power orbs in the game world for the sleight to collect
/// </summary>
public class PowerOrbSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private GameObject powerOrbPrefab;
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private int maxOrbs = 20;
    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private float minSpawnDistance = 10f;
    
    [Header("Spawn Area")]
    [SerializeField] private Vector3 spawnAreaCenter = Vector3.zero;
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(100f, 20f, 100f);
    [SerializeField] private LayerMask groundLayer = -1;
    [SerializeField] private float groundCheckDistance = 10f;
    
    [Header("Orb Types")]
    [SerializeField] private OrbSpawnData[] orbSpawnData;
    
    [Header("Dynamic Spawning")]
    [SerializeField] private bool followPlayer = true;
    [SerializeField] private float updateInterval = 1f;
    [SerializeField] private bool adaptiveSpawning = true;
    [SerializeField] private float adaptiveSpawnRateMultiplier = 1.5f;
    
    // Private fields
    private List<PowerOrb> spawnedOrbs = new List<PowerOrb>();
    private float lastSpawnTime = 0f;
    private Transform playerTransform;
    private SleightController sleightController;
    private float lastUpdateTime = 0f;
    
    void Start()
    {
        // Find player reference
        sleightController = FindFirstObjectByType<SleightController>();
        if (sleightController != null)
        {
            playerTransform = sleightController.transform;
        }
        
        // Initialize orb spawn data if not set
        if (orbSpawnData == null || orbSpawnData.Length == 0)
        {
            InitializeDefaultOrbSpawnData();
        }
        
        // Update spawn area center if following player
        if (followPlayer && playerTransform != null)
        {
            spawnAreaCenter = playerTransform.position;
        }
    }
    
    void Update()
    {
        UpdateSpawning();
        CleanupOrbs();
        
        // Update spawn area position
        if (followPlayer && playerTransform != null && Time.time - lastUpdateTime > updateInterval)
        {
            spawnAreaCenter = playerTransform.position;
            lastUpdateTime = Time.time;
        }
    }
    
    private void InitializeDefaultOrbSpawnData()
    {
        orbSpawnData = new OrbSpawnData[]
        {
            new OrbSpawnData { orbType = OrbType.Basic, weight = 50f },
            new OrbSpawnData { orbType = OrbType.Enhanced, weight = 30f },
            new OrbSpawnData { orbType = OrbType.Rare, weight = 15f },
            new OrbSpawnData { orbType = OrbType.Epic, weight = 4f },
            new OrbSpawnData { orbType = OrbType.Legendary, weight = 1f }
        };
    }
    
    private void UpdateSpawning()
    {
        if (powerOrbPrefab == null) return;
        
        // Check if we can spawn more orbs
        if (spawnedOrbs.Count >= maxOrbs) return;
        
        // Calculate spawn rate (adaptive based on player power level)
        float currentSpawnRate = spawnRate;
        if (adaptiveSpawning && sleightController != null)
        {
            float powerPercentage = sleightController.PowerLevel / sleightController.MaxPowerLevel;
            currentSpawnRate *= (1f + powerPercentage * adaptiveSpawnRateMultiplier);
        }
        
        // Check if it's time to spawn
        if (Time.time - lastSpawnTime >= 1f / currentSpawnRate)
        {
            SpawnPowerOrb();
            lastSpawnTime = Time.time;
        }
    }
    
    private void SpawnPowerOrb()
    {
        Vector3 spawnPosition = GetValidSpawnPosition();
        if (spawnPosition == Vector3.zero) return;
        
        // Create orb
        GameObject orbObj = Instantiate(powerOrbPrefab, spawnPosition, Quaternion.identity);
        PowerOrb orb = orbObj.GetComponent<PowerOrb>();
        
        if (orb != null)
        {
            // Set orb type based on weighted random selection
            OrbType orbType = GetRandomOrbType();
            orb.SetOrbType(orbType);
            
            // Add to spawned orbs list
            spawnedOrbs.Add(orb);
            
            // Set parent for organization
            orb.transform.SetParent(transform);
        }
    }
    
    private Vector3 GetValidSpawnPosition()
    {
        int attempts = 0;
        int maxAttempts = 30;
        
        while (attempts < maxAttempts)
        {
            // Generate random position within spawn area
            Vector3 randomPos = spawnAreaCenter + new Vector3(
                Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f),
                Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f),
                Random.Range(-spawnAreaSize.z * 0.5f, spawnAreaSize.z * 0.5f)
            );
            
            // Check if position is valid
            if (IsValidSpawnPosition(randomPos))
            {
                // Try to place on ground
                Vector3 groundPos = GetGroundPosition(randomPos);
                if (groundPos != Vector3.zero)
                {
                    return groundPos;
                }
                return randomPos;
            }
            
            attempts++;
        }
        
        return Vector3.zero; // No valid position found
    }
    
    private bool IsValidSpawnPosition(Vector3 position)
    {
        // Check distance from player
        if (playerTransform != null)
        {
            float distanceFromPlayer = Vector3.Distance(position, playerTransform.position);
            if (distanceFromPlayer < minSpawnDistance || distanceFromPlayer > spawnRadius)
            {
                return false;
            }
        }
        
        // Check for overlapping orbs
        foreach (PowerOrb orb in spawnedOrbs)
        {
            if (orb != null && Vector3.Distance(position, orb.transform.position) < 5f)
            {
                return false;
            }
        }
        
        return true;
    }
    
    private Vector3 GetGroundPosition(Vector3 position)
    {
        // Cast ray downward to find ground
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 5f, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            return hit.point + Vector3.up * 0.5f; // Slightly above ground
        }
        
        return Vector3.zero;
    }
    
    private OrbType GetRandomOrbType()
    {
        if (orbSpawnData == null || orbSpawnData.Length == 0)
        {
            return OrbType.Basic;
        }
        
        // Calculate total weight
        float totalWeight = 0f;
        foreach (OrbSpawnData data in orbSpawnData)
        {
            totalWeight += data.weight;
        }
        
        // Select random orb type based on weight
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        foreach (OrbSpawnData data in orbSpawnData)
        {
            currentWeight += data.weight;
            if (randomValue <= currentWeight)
            {
                return data.orbType;
            }
        }
        
        return OrbType.Basic; // Fallback
    }
    
    private void CleanupOrbs()
    {
        // Remove null or destroyed orbs from the list
        for (int i = spawnedOrbs.Count - 1; i >= 0; i--)
        {
            if (spawnedOrbs[i] == null)
            {
                spawnedOrbs.RemoveAt(i);
            }
        }
    }
    
    // Public methods
    public void SetSpawnRate(float rate)
    {
        spawnRate = Mathf.Max(0.1f, rate);
    }
    
    public void SetMaxOrbs(int max)
    {
        maxOrbs = Mathf.Max(1, max);
    }
    
    public void SetSpawnRadius(float radius)
    {
        spawnRadius = Mathf.Max(5f, radius);
    }
    
    public void SetSpawnAreaSize(Vector3 size)
    {
        spawnAreaSize = size;
    }
    
    public void ClearAllOrbs()
    {
        foreach (PowerOrb orb in spawnedOrbs)
        {
            if (orb != null)
            {
                Destroy(orb.gameObject);
            }
        }
        spawnedOrbs.Clear();
    }
    
    public void SpawnOrbAtPosition(Vector3 position, OrbType orbType = OrbType.Basic)
    {
        if (powerOrbPrefab == null) return;
        
        GameObject orbObj = Instantiate(powerOrbPrefab, position, Quaternion.identity);
        PowerOrb orb = orbObj.GetComponent<PowerOrb>();
        
        if (orb != null)
        {
            orb.SetOrbType(orbType);
            spawnedOrbs.Add(orb);
            orb.transform.SetParent(transform);
        }
    }
    
    public int GetSpawnedOrbCount()
    {
        return spawnedOrbs.Count;
    }
    
    public void SetFollowPlayer(bool follow)
    {
        followPlayer = follow;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw spawn area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
        
        // Draw spawn radius around player
        if (playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, spawnRadius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(playerTransform.position, minSpawnDistance);
        }
    }
}

[System.Serializable]
public class OrbSpawnData
{
    public OrbType orbType;
    public float weight = 1f;
}
