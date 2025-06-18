using UnityEngine;

public class basicmovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 5;
    public Rigidbody rb;
    private Vector3 input;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.z = Input.GetAxisRaw("Vertical");
        input = new Vector3(input.x, 0f, input.z);
        input = input.normalized;
        transform.Translate(input * speed * Time.deltaTime, Space.World);
    }
}
