using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    public CollectibleType type = CollectibleType.Coin;
    public int value = 1;
    public float bobSpeed = 2f;
    public float bobHeight = 0.5f;
    public float rotateSpeed = 90f;
    
    [Header("Effects")]
    public GameObject collectEffect;
    public AudioClip collectSound;
    
    private Vector3 startPosition;
    private AudioSource audioSource;
    
    public enum CollectibleType
    {
        Coin,
        Gem,
        Health,
        PowerUp
    }
    
    void Start()
    {
        startPosition = transform.position;
        audioSource = GetComponent<AudioSource>();
        
        // Add a trigger collider if none exists
        if (!GetComponent<Collider>())
        {
            var collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }
    }
    
    void Update()
    {
        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        
        // Rotate
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }
    
    void Collect(GameObject player)
    {
        // Play sound
        if (collectSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(collectSound);
        }
        
        // Spawn effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        
        // Handle collection based on type
        switch (type)
        {
            case CollectibleType.Coin:
                GameManager.Instance?.AddScore(value);
                break;
            case CollectibleType.Gem:
                GameManager.Instance?.AddScore(value * 5);
                break;
            case CollectibleType.Health:
                // Add health to player
                Debug.Log($"Player gained {value} health");
                break;
            case CollectibleType.PowerUp:
                // Give power-up to player
                Debug.Log($"Player gained power-up");
                break;
        }
        
        // Destroy the collectible
        Destroy(gameObject);
    }
}
