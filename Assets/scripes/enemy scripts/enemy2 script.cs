using UnityEngine;

public class enemy2script : MonoBehaviour
{
    public GameObject player;
    public enemybulletscript bulletPrefab; // Prefab of the bullet to be instantiated
    public Transform firePoint; // Point from where the bullet
    public Rigidbody enemyRb;
    public float health = 2;
    public float attackRange = 8f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float cooldownTime = 2f; // Time between shots
    private float lastShotTime = 0f;
    public bool hit = false;
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0.0f) // Check if the enemy's health is less than or equal to 0
        {
            Destroy(gameObject);
             // Destroy the enemy object
        }
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange) // Check if the enemy is close to the player
        {
        attackRange = 16f;
        Vector3 lookDirection = (player.transform.position - transform.position); // Move the enemy towards the player
        transform.LookAt(player.transform.position); 
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
        // Instantiate the bullet at the fire point
        if (Time.time >= lastShotTime + cooldownTime)
            {
                // Instantiate the bullet at the fire point
                enemybulletscript bullet1 = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                lastShotTime = Time.time; // Update the last shot time
            }
        }
    }
}
