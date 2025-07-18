using UnityEngine;

public class OffsetFollow : MonoBehaviour
{
    [Tooltip("The player's Transform to follow.")]
    public Transform player;

    public Vector3 offset;

    void Start() {
        offset = transform.position-player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = offset + player.transform.position;
    }
}
