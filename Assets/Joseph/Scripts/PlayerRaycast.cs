using UnityEngine;
using UnityEngine.UI;

public class PlayerRaycast : MonoBehaviour
{
    [Header("Raycasting")]
    public float rayDistance = 10f;
    public LayerMask ogreLayer;
    public LayerMask gunLayer;

    [Header("Gun Settings")]
    public Transform gunHoldPoint; // Where the gun will attach when picked up
    public GameObject bulletPrefab;
    public GameObject door;
    public Transform shootPoint;
    public float bulletSpeed = 20f;

    [Header("UI")]
    public GameObject crosshairUI;

    private GameObject heldGun;
    private bool isHoldingGun = false;
    private bool isHoldingKey = false;

    void Update()
    {
        // 1. Interact with Ogre
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, ogreLayer))
            {
                OgreScript ogre = hit.collider.GetComponent<OgreScript>();
                if (ogre != null && !isHoldingGun)
                {
                    Debug.Log("Player clicked on ogre. Calling DropGun()");
                    ogre.DropGun();
                }
            }
        }

        // 2. Pick up gun (on key E)
        if (!isHoldingGun && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed - looking for gun");
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            //if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, gunLayer))
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                Debug.Log("Hit object: " + hit.collider.name);
                if (hit.collider.CompareTag("Gun"))
                {
                    PickUpGun(hit.collider.gameObject);
                }
            }
        }

        if (!isHoldingKey && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed - looking for key");
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));


            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                Debug.Log("Hit object: " + hit.collider.name);
                if (hit.collider.CompareTag("Key"))
                {
                    Debug.Log("Added key to pockets");
                    //PickUpKey(hit.collider.gameObject);
                    Destroy(hit.collider.gameObject);
                    isHoldingKey = true;
                }
            }
        }

        /*if (isHoldingKey && Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
            {
                Debug.Log("Hit object: " + hit.collider.name);
                if (hit.collider.CompareTag("Door"))
                {
                    Debug.Log("Opening Door");
                    //OpenDoor();

                }
            }*/


            // 3. Shoot gun
            if (isHoldingGun && Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        void PickUpGun(GameObject gun)
        {
            heldGun = gun;
            isHoldingGun = true;

            // Move gun to hold point
            gun.transform.SetParent(gunHoldPoint);
            gun.transform.localPosition = Vector3.zero;
            gun.transform.localRotation = Quaternion.identity;

            // Disable physics
            Rigidbody rb = gun.GetComponent<Rigidbody>();
            if (rb) rb.isKinematic = true;

            Collider col = gun.GetComponent<Collider>();
            if (col) col.enabled = false;

            // Enable crosshair
            if (crosshairUI) crosshairUI.SetActive(true);

            Debug.Log("Gun picked up!");
        }

        void Shoot()
        {
            if (bulletPrefab && shootPoint)
            {
                GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb) rb.linearVelocity = shootPoint.forward * bulletSpeed;
                //Debug.Log("Bang!");
            }
        }
        /*void OpenDoor()
        {
            transform.rotation = Quaternion.Euler(0, -75, 0);
        }*/
    }

