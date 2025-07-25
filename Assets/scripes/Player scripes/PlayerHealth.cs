using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    public HealthBar healthBar;

    public string dreamWorldName = "Dream1";

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

            //Load Dream scene
            SceneManager.LoadScene(dreamWorldName);
        }
    }
}
