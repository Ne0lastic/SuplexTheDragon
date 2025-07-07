using UnityEngine;

public class BallScript : MonoBehaviour
{
    public GateController gateController;

    private bool hasTriggeredGate = false;

    public void OpenGate()
    {
        if (hasTriggeredGate) return;
        hasTriggeredGate = true;

        if (gateController != null)
        {
            gateController.OpenGate();
            Debug.Log("Ball triggered the gate to open!");
        }

        Destroy(gameObject); // Remove the ball
    }
}


