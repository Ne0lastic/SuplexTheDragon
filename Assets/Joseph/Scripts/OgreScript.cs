using UnityEngine;

public class OgreScript : MonoBehaviour
{
    [Header("Key Reference")]
    public GameObject GunObject;        // The key GameObject
    public Transform dropPoint;         // Where the key should be dropped from

    //private bool hasDroppedGun = false;

    void Start()
    {
        // Make sure key starts attached to ogre and can't move
        if (GunObject.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }

        if (GunObject.TryGetComponent(out Collider col))
        {
            col.enabled = false;
        }
    }

    void Update()
    {
        // Example condition: player is within 5 units
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        /*if (!hasDroppedGun && player && Vector3.Distance(player.transform.position, transform.position) < 5f)
        {
            DropGun();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            DropGun();
        }*/
    }

    public void DropGun()
    {
        //hasDroppedGun = true;

        // Detach the key
        GunObject.transform.SetParent(null);

        // Move it to drop point
        GunObject.transform.position = dropPoint.position;

        // Enable physics
        Rigidbody rb = GunObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        Collider col = GunObject.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        GetComponent<AudioSource>().Play();


        Debug.Log("Ogre dropped the gun!");
    }
}
