using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

public class enemy3script : MonoBehaviour
{
     public float speed = 3f; // Speed of the enemy
    public Rigidbody enemyRb; // Reference to the Rigidbody component
    public GameObject player;
    public bool hit = false;
    public float health = 1; 
    public float attackRange = 8f; // Range within which the enemy can attack the player
    // Flag to check if the enemy has hit the player
    private NavMeshAgent agent; // Reference to the NavMeshAgent component
    public Renderer[] rend;
    public Color flashColor = Color.red; // Color to flash when hit
    public AudioClip diesound;
    private AudioSource PlayerAudio;
    private Color[] origColors;
    public float flashTime = 0.15f;
    public GameObject explosionRingPrefab; // Assign a ring prefab in the inspector
    private bool explosionHasHit = false; // Prevents multiple hits per explosion
    public ParticleSystem effect; // Particle effect to play on death



     // Reference to the player object
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyRb = GetComponent<Rigidbody>(); 
        agent = GetComponent<NavMeshAgent>();
        origColors = new Color[rend.Length];
        effect = GetComponent<ParticleSystem>();

    for (int i = 0; i < rend.Length; i++)
        {
            origColors[i] = rend[i].material.color;
        } // Store the original color
         // Get the NavMeshAgent component attached to the enemy
         PlayerAudio = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            health -= 1; // Decrease health by 1 when right mouse button is pressed
        }
        if (health <= 0.0f) // Check if the enemy's health is less than or equal to 0
            {
                agent.isStopped = true;
                // Freeze the Rigidbody to stop all movement
                if (enemyRb != null)
                {
                    enemyRb.linearVelocity = Vector3.zero;
                    enemyRb.angularVelocity = Vector3.zero;
                    enemyRb.isKinematic = true;
                    enemyRb.constraints = RigidbodyConstraints.FreezeAll;
                }
                if (explosionRingPrefab != null && hit == false) // Check if the explosion ring prefab is assigned and the enemy hasn't exploded yet
                {
                    GameObject ring = Instantiate(explosionRingPrefab, transform.position + Vector3.up * 0.1f, Quaternion.Euler(0, 0, 0));
                    float ringRadius = .5f; // Match SphereCollider radius
                    Destroy(ring, 1.5f); // Ring lasts 1.5 seconds
                    hit = true; // Set the hit flag to true
                }
                StartCoroutine(explosion());
            }
        // Rotate the enemy to face the player
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange) // Check if the enemy is close to the player
        {

            attackRange = 16f;
            Vector3 lookDirection = (player.transform.position - transform.position);
            agent.destination = player.transform.position; // Move the enemy towards the player
            transform.LookAt(player.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y - 90, 0); // Keep the enemy upright
        }
        
    }

 public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && hit == false)
        {
            hit = true; // Set the hit flag to true
            StartCoroutine(hitcooldown()); // Start the cooldown coroutine
            PlayerHealth playerMovement = other.gameObject.GetComponent<PlayerHealth>();
            playerMovement.TakeDamage(2);
            Vector3 awayFromPlayer = other.gameObject.transform.position - transform.position;
            Vector3 awayfromenemy = transform.position - other.gameObject.transform.position;
            
            awayFromPlayer.y = 0; // Set y component to 0 to keep the enemy on the ground
            awayfromenemy.y = 0; // Set y component to 0 to keep the player on the ground
            enemyRb.AddForce(awayFromPlayer * -10, ForceMode.Impulse);
            
        }
        
    }
    IEnumerator hitcooldown() 
    {
        yield return new WaitForSeconds(1); 
        hit = false; 
    }
    private IEnumerator doflash()
    {
        // Change the color of all renderers to the flash color
        foreach (Renderer renderer in rend)
        {
            renderer.material.color = flashColor;
        }
        yield return new WaitForSeconds(flashTime); // Wait for the specified flash time

        // Change the color of all renderers back to their original colors
        for (int i = 0; i < rend.Length; i++)
        {
            rend[i].material.color = origColors[i];
        }
    }
    IEnumerator explosion()
    {
        if (explosionHasHit) yield break; // Prevent running more than once
        explosionHasHit = true;
        yield return new WaitForSeconds(1.5f);
        agent.isStopped = true; // Stop the enemy from moving

        float ringRadius = 5f; // Set the radius for both the hitbox and the ring

        // Create a circular hitbox for the explosion
        SphereCollider explosionCollider = gameObject.AddComponent<SphereCollider>();
        explosionCollider.radius = ringRadius; // Match the ring's radius
        explosionCollider.isTrigger = true; // Make it a trigger collider

        // Wait for a short time to allow the trigger detection
        yield return new WaitForSeconds(0.1f);

        // Only hit the player
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ringRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerHealth playerMovement = hitCollider.GetComponent<PlayerHealth>();
                if (playerMovement != null)
                {
                    playerMovement.TakeDamage(2); // Or any damage value you want
                    Vector3 awayFromExplosion = hitCollider.transform.position - transform.position;
                    awayFromExplosion.y = 0; // Set y component to 0 to keep the player on the ground
                    hitCollider.GetComponent<Rigidbody>().AddForce(awayFromExplosion.normalized * 10f, ForceMode.Impulse); // Apply knockback force
                }
            }
        }
        // Play death effect on explosion
        
        // Disable all mesh renderers so the enemy is invisible after explosion
        foreach (Renderer renderer in rend)
        {
            renderer.enabled = false;
        }
        // Wait a short time to allow trigger detection, then disable and destroy collider and game object
        Destroy(explosionCollider, 0.01f);
        if (effect != null)
        {
            effect.Play();
        }
         // Disable the enemy game object
        Destroy(gameObject, 2f);
    }

    // Removed OnTriggerEnter to ensure explosion only hits once
    public void flashStart(){
        StopCoroutine(doflash()); // Stop any existing flash coroutines
        StartCoroutine(doflash()); // Start the flash coroutine
    }
}
