using UnityEditor;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth); // Ensure slider reflects initial health
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage) // Made public for external access
    {
        currentHealth -= damage; // Reduces health by exact damage amount (1 from bullet)
        healthBar.SetHealth(currentHealth);

        // Check for player death
        if (currentHealth <= 0)
        {
            currentHealth = 0; // Prevent negative health
            healthBar.SetHealth(currentHealth);
            Debug.Log("Player is dead!");
            Destroy(gameObject);
            // Add game over or respawn logic here
        }
    }
}
