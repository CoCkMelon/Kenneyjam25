using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public PlatformType platformType = PlatformType.BackAndForth;
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waitTime = 1f;

    [Header("Falling Platform Settings")]
    public float fallDelay = 1f;
    public float respawnTime = 3f;

    [Header("Rotation Settings")]
    public bool rotateWithMovement = false;
    public float rotationSpeed = 90f;

    private int currentWaypoint = 0;
    private bool movingForward = true;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    // Falling platform variables
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isFalling = false;
    private bool hasPlayer = false;
    private float fallTimer = 0f;
    private Rigidbody rb;
    private Collider col;

    public enum PlatformType
    {
        BackAndForth,
        Circular,
        Falling,
        Rotating
    }

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Set up rigidbody for falling platforms
        if (platformType == PlatformType.Falling && rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        // Validate waypoints
        if (waypoints == null || waypoints.Length < 2)
        {
            if (platformType == PlatformType.BackAndForth || platformType == PlatformType.Circular)
            {
                Debug.LogWarning($"Moving platform {gameObject.name} needs at least 2 waypoints!");

            }
        }
    }

    void Update()
    {
        switch (platformType)
        {
            case PlatformType.BackAndForth:
                MoveBackAndForth();
                break;
            case PlatformType.Circular:
                MoveCircular();
                break;
            case PlatformType.Falling:
                HandleFallingPlatform();
                break;
            case PlatformType.Rotating:
                RotatePlatform();
                break;
        }
    }

    void MoveBackAndForth()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
            }
            return;
        }

        Transform target = waypoints[currentWaypoint];

        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Rotate if enabled
        if (rotateWithMovement)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            isWaiting = true;

            if (movingForward)
            {
                currentWaypoint++;
                if (currentWaypoint >= waypoints.Length)
                {
                    currentWaypoint = waypoints.Length - 2;
                    movingForward = false;
                }
            }
            else
            {
                currentWaypoint--;
                if (currentWaypoint < 0)
                {
                    currentWaypoint = 1;
                    movingForward = true;
                }
            }
        }
    }

    void MoveCircular()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
            }
            return;
        }

        Transform target = waypoints[currentWaypoint];

        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            isWaiting = true;
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }
    }

    void HandleFallingPlatform()
    {
        if (isFalling) return;

        if (hasPlayer)
        {
            fallTimer += Time.deltaTime;
            if (fallTimer >= fallDelay)
            {
                StartFalling();
            }
        }
    }

    void StartFalling()
    {
        isFalling = true;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
        }

        // Respawn after delay
        Invoke(nameof(RespawnPlatform), respawnTime);
    }

    void RespawnPlatform()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (col != null)
        {
            col.enabled = true;
        }

        isFalling = false;
        hasPlayer = false;
        fallTimer = 0f;
    }

    void RotatePlatform()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hasPlayer = true;

            // Make player a child of platform for moving platforms
            if (platformType == PlatformType.BackAndForth || platformType == PlatformType.Circular)
            {
                collision.transform.SetParent(transform);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            hasPlayer = false;
            fallTimer = 0f;

            // Remove player from platform
            if (collision.transform.parent == transform)
            {
                collision.transform.SetParent(null);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (waypoints == null) return;

        Gizmos.color = Color.yellow;

        // Draw waypoints
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);

                // Draw connections
                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }

                // For circular platforms, connect last to first
                if (platformType == PlatformType.Circular && i == waypoints.Length - 1 && waypoints[0] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                }
            }
        }
    }
}
