using UnityEngine;

public class sword : MonoBehaviour
{
    public basicmovement playerMovement; // Reference to the player movement script
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         playerMovement = GetComponent<basicmovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy1")
        {
            enemyscript enemyS = other.gameObject.GetComponent<enemyscript>();
            if (enemyS != null && enemyS.enemyRb != null)
            {
                enemyS.takedamage(1.0f); // Call the takedamage method on the enemy script

                // Calculate the knockback direction
                Vector3 awayFromPlayer = enemyS.transform.position - transform.position;
                awayFromPlayer.y = 0; // Keep the force horizontal
                awayFromPlayer.Normalize(); // Normalize the direction vector

                // Apply knockback force
                enemyS.enemyRb.AddForce(awayFromPlayer * 20f, ForceMode.Impulse);

                Debug.Log("Hit enemy and applied knockback");
            }
        }
        else if (other.gameObject.tag == "enemy2")
        {
            enemy2script enemyP = other.gameObject.GetComponent<enemy2script>();
            if (enemyP != null && enemyP.enemyRb != null)
            {
                enemyP.takedamage(1.0f); // Call the takedamage method on the enemy script

                // Calculate the knockback direction
                Vector3 awayFromPlayer = enemyP.transform.position - transform.position;
                awayFromPlayer.y = 0; // Keep the force horizontal
                awayFromPlayer.Normalize(); // Normalize the direction vector

                // Apply knockback force
                enemyP.enemyRb.AddForce(awayFromPlayer * 20f, ForceMode.Impulse);

                Debug.Log("Hit enemy and applied knockback");
            }
        }
        else if (other.gameObject.tag == "enemy3")
        {
            enemy3script enemyE = other.gameObject.GetComponent<enemy3script>();
            if (enemyE != null && enemyE.enemyRb != null)
            {
                enemyE.takedamage(1.0f); // Call the takedamage method on the enemy script

                // Calculate the knockback direction
                Vector3 awayFromPlayer = enemyE.transform.position - transform.position;
                awayFromPlayer.y = 0; // Keep the force horizontal
                awayFromPlayer.Normalize(); // Normalize the direction vector

                Debug.Log("Hit enemy and applied knockback");
            }
        }
        
    }
}
