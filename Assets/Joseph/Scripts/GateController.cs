using UnityEngine;

public class GateController : MonoBehaviour
{
    public float openSpeed = 2f;
    public float openAmount = 7f;

    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool isOpening = false;

    void Start()
    {
        closedPosition = transform.position;
        targetPosition = closedPosition + new Vector3(0, openAmount, 0);
    }

    void Update()
    {
        if (isOpening)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                openSpeed * Time.deltaTime
            );
        }
    }

    public void OpenGate()
    {
        isOpening = true;
        Debug.Log("Gate is opening upward.");
    }
}
