using UnityEngine;

public class enemybulletscript : MonoBehaviour
{
     public float speed = 12f;
    public float bulletLifeTime = 1f;
    public int bulletDamage = 1;
    public float bulletKnockback = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        bulletLifeTime -= Time.deltaTime;
        if (bulletLifeTime <= 0)
        {
            Destroy(gameObject);
        }
        

    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) // Fixed tag to "Player"
        {
            // Apply damage
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(bulletDamage);
                Debug.Log($"Bullet dealt {bulletDamage} damage to player. Player health: {playerHealth.currentHealth}");
            }

            // Apply knockback
            Rigidbody playerRigidbody = other.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
                playerRigidbody.AddForce(knockbackDirection * bulletKnockback, ForceMode.Impulse);
            }

            // Destroy bullet on player hit
            Destroy(gameObject);
        }
        else if (other.gameObject.CompareTag("Level"))
        {
            Destroy(gameObject);
        }
    }
}
