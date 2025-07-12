using UnityEngine;

public class basicmovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 5;
    public Rigidbody rb;
    private Vector3 input;
    private Animator anim;
    private int slashC;
    public Camera gameCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        slashC = Animator.StringToHash("slashing");
        rb = GetComponent<Rigidbody>();
        if (gameCamera == null)
        {
            gameCamera = Camera.main;
            if (gameCamera == null)
            {
                Debug.LogError("Main Camera not found! Ensure the camera has the 'MainCamera' tag.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.z = Input.GetAxisRaw("Vertical");
        input = new Vector3(input.x, 0f, input.z);
        input = input.normalized;
        transform.Translate(input * speed * Time.deltaTime, Space.World);
        lookAtMouse();
        Slash();
    }
    public void Slash()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool(slashC, true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            anim.SetBool(slashC, false);
        }
    }
     public void lookAtMouse()
    {
        Ray cameraRay = gameCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Vector3 direction = pointToLook - transform.position;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation * Quaternion.Euler(0, 0, 0);
            }

            Debug.DrawLine(cameraRay.origin, pointToLook, Color.green);
        }
        else
        {
            Debug.LogWarning("Ray did not hit the ground plane.");
        }
    }
}
