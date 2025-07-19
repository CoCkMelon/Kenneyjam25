using UnityEngine;

/// <summary>
/// Moves the floor closer to the player in XZ axis when the player is further than a specified distance.
/// The floor moves in chunks (e.g., to 1000,0,0 then 2000,0,0 etc.) to stay close to the player.
/// </summary>
public class FloorFollower : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform floor;
    
    [Header("Movement Settings")]
    [SerializeField] private float chunkSize = 1000f; // Size of each floor chunk
    [SerializeField] private float triggerDistance = 500f; // Distance from chunk center to trigger move
    [SerializeField] private bool smoothMovement = true;
    [SerializeField] private float moveSpeed = 10f; // Speed for smooth movement
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    private Vector3 targetPosition;
    private bool isMoving = false;
    
    void Start()
    {
        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning("FloorFollower: No player reference assigned and no GameObject with 'Player' tag found!");
        }
        
        // Auto-assign this gameobject as floor if not assigned
        if (floor == null)
            floor = transform;
        
        // Initialize target position to current floor position
        targetPosition = floor.position;
    }
    
    void Update()
    {
        if (player == null || floor == null) return;
        
        CheckPlayerDistance();
        
        if (smoothMovement && isMoving)
        {
            MoveSmoothly();
        }
    }
    
    private void CheckPlayerDistance()
    {
        // Get current floor chunk center based on floor position
        Vector3 floorCenter = new Vector3(floor.position.x, floor.position.y, floor.position.z);
        
        // Calculate distance from player to floor center in XZ plane
        Vector3 playerPosXZ = new Vector3(player.position.x, floor.position.y, player.position.z);
        Vector3 floorCenterXZ = new Vector3(floorCenter.x, floor.position.y, floorCenter.z);
        float distanceXZ = Vector3.Distance(playerPosXZ, floorCenterXZ);
        
        if (showDebugInfo)
        {
            Debug.Log($"Player distance from floor center: {distanceXZ:F2} (trigger at {triggerDistance})");
        }
        
        // Check if player is far enough to trigger floor movement
        if (distanceXZ > triggerDistance)
        {
            MoveFloorTowardsPlayer();
        }
    }
    
    private void MoveFloorTowardsPlayer()
    {
        // Calculate which chunk the player should be in
        float playerChunkX = Mathf.Round(player.position.x / chunkSize) * chunkSize;
        float playerChunkZ = Mathf.Round(player.position.z / chunkSize) * chunkSize;
        
        // Set target position maintaining the floor's Y position
        Vector3 newTargetPosition = new Vector3(playerChunkX, floor.position.y, playerChunkZ);
        
        // Only move if the target position is different
        if (Vector3.Distance(newTargetPosition, targetPosition) > 0.1f)
        {
            targetPosition = newTargetPosition;
            
            if (smoothMovement)
            {
                isMoving = true;
            }
            else
            {
                // Instant movement
                floor.position = targetPosition;
                if (showDebugInfo)
                {
                    Debug.Log($"Floor moved instantly to: {targetPosition}");
                }
            }
        }
    }
    
    private void MoveSmoothly()
    {
        float step = moveSpeed * Time.deltaTime;
        floor.position = Vector3.MoveTowards(floor.position, targetPosition, step);
        
        // Check if we've reached the target
        if (Vector3.Distance(floor.position, targetPosition) < 0.01f)
        {
            floor.position = targetPosition;
            isMoving = false;
            
            if (showDebugInfo)
            {
                Debug.Log($"Floor reached target position: {targetPosition}");
            }
        }
    }
    
    // Public method to manually set the floor to a specific chunk
    public void SetFloorToChunk(float chunkX, float chunkZ)
    {
        targetPosition = new Vector3(chunkX * chunkSize, floor.position.y, chunkZ * chunkSize);
        
        if (smoothMovement)
        {
            isMoving = true;
        }
        else
        {
            floor.position = targetPosition;
        }
    }
    
    // Public method to get current chunk coordinates
    public Vector2 GetCurrentChunk()
    {
        return new Vector2(
            Mathf.Round(floor.position.x / chunkSize),
            Mathf.Round(floor.position.z / chunkSize)
        );
    }
    
    void OnDrawGizmosSelected()
    {
        if (floor == null) return;
        
        // Draw floor chunk boundaries
        Gizmos.color = Color.green;
        Vector3 chunkCenter = new Vector3(
            Mathf.Round(floor.position.x / chunkSize) * chunkSize,
            floor.position.y,
            Mathf.Round(floor.position.z / chunkSize) * chunkSize
        );
        
        // Draw chunk area
        Gizmos.DrawWireCube(chunkCenter, new Vector3(chunkSize, 1f, chunkSize));
        
        // Draw trigger distance circle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(chunkCenter, triggerDistance);
        
        if (player != null)
        {
            // Draw line from floor center to player
            Gizmos.color = Color.red;
            Gizmos.DrawLine(chunkCenter, new Vector3(player.position.x, floor.position.y, player.position.z));
        }
    }
}
