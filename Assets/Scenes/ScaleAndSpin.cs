using UnityEngine;

public class ScaleAndSpin : MonoBehaviour
{
    public float scaleSpeed = 1f;       // Speed of scaling
    public float rotationSpeed = 90f;   // Degrees per second
    public float minScale = 0.1f;       // 10%
    public float maxScale = 2f;         // 200%

    private bool scalingUp = true;

    void Update()
    {
        // ROTATE around the X axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // SCALE up and down between minScale and maxScale
        float scaleChange = scaleSpeed * Time.deltaTime;
        Vector3 newScale = transform.localScale;

        if (scalingUp)
        {
            newScale += Vector3.one * scaleChange;
            if (newScale.x >= maxScale)
                scalingUp = false;
        }
        else
        {
            newScale -= Vector3.one * scaleChange;
            if (newScale.x <= minScale)
                scalingUp = true;
        }

        // Apply uniform scale
        newScale = Vector3.one * Mathf.Clamp(newScale.x, minScale, maxScale);
        transform.localScale = newScale;
    }
}
