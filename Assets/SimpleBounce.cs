using UnityEngine;

public class SimpleBounce : MonoBehaviour
{
    public float amplitude = 1f; // How high it moves
    public float frequency = 1f; // Speed of movement

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position; // Save initial position
    }

    void Update()
    {
        // Calculate new Y position using sine wave
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // Update object position
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
