// ✅ PaladinController.cs (Fixed attack height and damage against short goblins)
using UnityEngine;
using UnityEngine.UI;

public class PaladinController : MonoBehaviour
{
    public Rigidbody rb;
    public Animation animationComponent;
    public AudioSource swordAudioSource;
    public AudioClip swordSwingSound;

    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 8f;
    public float attackRange = 2f;
    public float attackRadius = 1.5f;
    public float maxHitPoints = 100f;
    public float currentHealth;
    public float attackCooldown = 0.8f;
    public float healthRegenRate = 2f;
    public float damage = 20f;
    public float groundCheckDistance = 1.1f;

    public RectTransform healthBarRect;
    private float originalHealthBarWidth;

    public string idleAnimation = "Idle";
    public string walkAnimation = "Walking";
    public string runAnimation = "Running";
    public string jumpAnimation = "Jumping";
    public string deathAnimation = "Death";
    public string attack1Animation = "Attack1";
    public string attack2Animation = "Attack2";
    public string attack3Animation = "Attack3";
    public string attack4Animation = "Attack4";

    private bool isGrounded = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool isJumping = false;
    private int attackStep = 0;
    private float lastAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animationComponent = GetComponent<Animation>();
        swordAudioSource = GetComponent<AudioSource>();

        if (healthBarRect != null)
            originalHealthBarWidth = healthBarRect.sizeDelta.x;

        currentHealth = maxHitPoints;
        ConfigureAnimations();
        PlayIdleAnimation();
        UpdateHealthUI();
        InvokeRepeating(nameof(RegenerateHealth), 1f, 1f);
    }

    void ConfigureAnimations()
    {
        animationComponent[walkAnimation].wrapMode = WrapMode.Loop;
        animationComponent[runAnimation].wrapMode = WrapMode.Loop;
        animationComponent[idleAnimation].wrapMode = WrapMode.Loop;
        animationComponent[jumpAnimation].wrapMode = WrapMode.Once;
        animationComponent[attack1Animation].wrapMode = WrapMode.Once;
        animationComponent[attack2Animation].wrapMode = WrapMode.Once;
        animationComponent[attack3Animation].wrapMode = WrapMode.Once;
        animationComponent[attack4Animation].wrapMode = WrapMode.Once;
        animationComponent[deathAnimation].wrapMode = WrapMode.Once;
    }

    void Update()
    {
        if (isDead) return;
        CheckGroundStatus();
        HandleMovement();
        HandleAttack();
    }

    void CheckGroundStatus()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        if (isGrounded) isJumping = false;
    }

    void HandleMovement()
    {
        if (isAttacking || isJumping) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0, v).normalized;

        if (input.magnitude > 0.1f)
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            Vector3 moveDir = transform.TransformDirection(input) * speed;
            rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);

            if (Input.GetKey(KeyCode.LeftShift))
                animationComponent.Play(runAnimation);
            else
                animationComponent.Play(walkAnimation);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            PlayIdleAnimation();
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animationComponent.Play(jumpAnimation);
            isJumping = true;
        }
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            attackStep = (attackStep % 4) + 1;

            string anim = attackStep switch
            {
                1 => attack1Animation,
                2 => attack2Animation,
                3 => attack3Animation,
                4 => attack4Animation,
                _ => attack1Animation
            };

            rb.linearVelocity = Vector3.zero;
            animationComponent.Play(anim);
            PlaySwordSound();
            DealDamage();

            Invoke(nameof(EndAttack), animationComponent[anim].length * 0.9f);
        }
    }

    void DealDamage()
    {
        // Lower the sphere center to hit short goblins
        Vector3 hitOrigin = transform.position + Vector3.up * 0.5f;
        Collider[] hits = Physics.OverlapSphere(hitOrigin, attackRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                var enemy = hit.GetComponent<EnemyKnightController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        PlayIdleAnimation();
    }

    void PlayIdleAnimation()
    {
        if (!animationComponent.IsPlaying(idleAnimation))
            animationComponent.Play(idleAnimation);
    }

    void PlaySwordSound()
    {
        if (swordAudioSource != null && swordSwingSound != null)
            swordAudioSource.PlayOneShot(swordSwingSound);
    }

    void RegenerateHealth()
    {
        if (!isDead && currentHealth < maxHitPoints)
        {
            currentHealth += healthRegenRate;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHitPoints);
            UpdateHealthUI();
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return; // Prevent damage & animations if already dead

        currentHealth -= amount;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            isDead = true;
            animationComponent.Play(deathAnimation);
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true; // Optional: freeze movement
        }
    }


    void UpdateHealthUI()
    {
        if (healthBarRect != null)
        {
            float percent = currentHealth / maxHitPoints;
            healthBarRect.sizeDelta = new Vector2(originalHealthBarWidth * percent, healthBarRect.sizeDelta.y);
        }
    }
}
