using UnityEngine;

public class SuplexController : MonoBehaviour
{
    public Transform dragon; // Assign dragon GameObject in Inspector
    public Transform ground; // Assign ground GameObject in Inspector
    public float jumpSpeed = 5f; // Speed of warrior's jump to dragon
    public float arcHeight = 10f; // Height of suplex arc
    public float arcDuration = 1f; // Time for suplex arc
    public float bounceForce = 5f; // Force of dragon's bounce
    public float warriorLandSpeed = 2f; // Speed for warrior to return upright
    public AudioClip whooshClip; // Whooshing sound for jump
    public AudioClip grabClip; // Grab sound
    public AudioClip fwooshClip; // Fwoosh sound for arc
    public AudioClip crackleClip; // Crackle sound for impact
    public AudioClip bangClip; // Bang sound for impact
    public AudioClip thudClip; // Thud sound for impact

    private Rigidbody warriorRb;
    private Rigidbody dragonRb;
    private AudioSource audioSource;
    private enum State { Idle, Jumping, Grabbing, Suplexing, Impact, Landing }
    private State currentState = State.Idle;
    private Vector3 startPos;
    private Vector3 behindDragonPos;
    private float arcTimer = 0f;
    private bool hasCollided = false;

    void Start()
    {
        warriorRb = GetComponent<Rigidbody>();
        dragonRb = dragon.GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        startPos = transform.position;

        // Ensure AudioSource is present
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource component missing on " + gameObject.name + ". Adding one.");
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Start sequence with a key press (e.g., Space), only if fully idle
        if (Input.GetKeyDown(KeyCode.Space) && currentState == State.Idle && !hasCollided)
        {
            currentState = State.Jumping;
            PlaySound(whooshClip);
        }

        switch (currentState)
        {
            case State.Idle:
                // No action needed in Idle state
                break;
            case State.Jumping:
                JumpToDragon();
                break;
            case State.Grabbing:
                GrabDragon();
                break;
            case State.Suplexing:
                PerformSuplex();
                break;
            case State.Impact:
                // Wait for collision to handle impact (handled in OnCollisionEnter)
                break;
            case State.Landing:
                LandWarrior();
                break;
        }
    }

    void JumpToDragon()
    {
        // Calculate position behind dragon (2 units behind along dragon's forward axis)
        behindDragonPos = dragon.position - dragon.forward * 2f;
        behindDragonPos.y = dragon.position.y; // Align at dragon's height

        // Move warrior towards behindDragonPos using Rigidbody
        warriorRb.MovePosition(Vector3.MoveTowards(transform.position, behindDragonPos, jumpSpeed * Time.deltaTime));

        // Look at dragon while moving
        transform.LookAt(dragon);

        // Check if warrior has reached the position
        if (Vector3.Distance(transform.position, behindDragonPos) < 0.1f)
        {
            currentState = State.Grabbing;
            PlaySound(grabClip);
        }
    }

    void GrabDragon()
    {
        // Attach warrior to dragon (parenting for simplicity)
        transform.SetParent(dragon);
        transform.localPosition = new Vector3(0f, 1f, -1f); // Slightly above and behind dragon
        transform.localRotation = Quaternion.identity;

        // Proceed to suplex
        currentState = State.Suplexing;
        arcTimer = 0f;
        PlaySound(fwooshClip);

        // Unfreeze dragon's rotation constraints for suplex
        dragonRb.constraints = RigidbodyConstraints.None;
    }

    void PerformSuplex()
    {
        arcTimer += Time.deltaTime;
        float t = arcTimer / arcDuration; // Normalized time (0 to 1)

        if (t <= 1f)
        {
            // Create parabolic arc: warrior and dragon move up, then down head-first
            Vector3 startPos = dragon.position;
            Vector3 endPos = new Vector3(dragon.position.x, ground.position.y, dragon.position.z);
            float height = arcHeight * (1f - 4f * (t - 0.5f) * (t - 0.5f)); // Parabolic height
            Vector3 arcPos = Vector3.Lerp(startPos, endPos, t); // Linearly interpolate X and Z
            arcPos.y += height; // Add parabolic height for Y
            dragonRb.MovePosition(arcPos);

            // Rotate dragon and warrior 180 degrees (head-first)
            float angle = Mathf.Lerp(0f, 180f, t);
            dragonRb.MoveRotation(Quaternion.Euler(angle, dragon.eulerAngles.y, dragon.eulerAngles.z));
        }
        else if (!hasCollided)
        {
            // Trigger impact when arc completes
            currentState = State.Impact;
            PlayImpactSounds();
        }
    }

    void PlayImpactSounds()
    {
        // Play crackle, bang, thud sequentially
        StartCoroutine(PlaySequentialSounds());
    }

    System.Collections.IEnumerator PlaySequentialSounds()
    {
        if (crackleClip != null)
        {
            audioSource.PlayOneShot(crackleClip);
            yield return new WaitForSeconds(crackleClip.length);
        }
        if (bangClip != null)
        {
            audioSource.PlayOneShot(bangClip);
            yield return new WaitForSeconds(bangClip.length);
        }
        if (thudClip != null)
        {
            audioSource.PlayOneShot(thudClip);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && currentState == State.Impact && !hasCollided)
        {
            hasCollided = true;

            // Detach warrior from dragon
            transform.SetParent(null);

            // Apply bounce to dragon
            dragonRb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);

            // Rotate dragon to lie on its side (90 degrees on Z-axis)
            dragonRb.MoveRotation(Quaternion.Euler(dragon.eulerAngles.x, dragon.eulerAngles.y, 90f));

            // Transition to landing state
            currentState = State.Landing;
        }
    }

    void LandWarrior()
    {
        // Move warrior to ground level
        Vector3 targetPos = new Vector3(transform.position.x, ground.position.y + 1f, transform.position.z);
        warriorRb.MovePosition(Vector3.MoveTowards(transform.position, targetPos, warriorLandSpeed * Time.deltaTime));

        // Rotate warrior back to upright
        Quaternion targetRot = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        warriorRb.MoveRotation(Quaternion.Lerp(transform.rotation, targetRot, warriorLandSpeed * Time.deltaTime));

        // Check if warrior is upright and on ground
        if (Vector3.Distance(transform.position, targetPos) < 0.1f && Quaternion.Angle(transform.rotation, targetRot) < 1f)
        {
            currentState = State.Idle; // Ready for next suplex
            hasCollided = false;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}