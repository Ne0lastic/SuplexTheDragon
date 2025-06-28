using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class AnimationSet
{
    public string idle;
    public string walk;
    public string run;
    public string death;
    public string jump;
    public string[] attacks = new string[4];
}

public class EnemyKnightController : MonoBehaviour
{
    public Animation animationComponent;
    public AnimationSet animations;

    public AudioSource swordAudioSource;
    public AudioSource painAudioSource;
    public AudioSource deathAudioSource;
    public AudioSource attackAudioSource;

    public AudioClip[] swordSwingSounds;
    public AudioClip[] swordHitSounds;
    public AudioClip[] attackSounds;
    public AudioClip[] painSounds;
    public AudioClip[] deathSounds;

    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float attackRange = 2f;
    public float attackRadius = 2f;
    public float attackCooldown = 1.5f;
    public float detectionRange = 10f;
    public float wanderRadius = 10f;
    public float wanderCooldown = 5f;
    public float maxHitPoints = 50f;
    public float healthRegenRate = 1f;
    public float damage = 15f;

    public Transform modelRoot; // Assign mesh/model here
    public bool flipModelUpsideDown = false;
    public bool rotateModelBackwards = false;

    private float currentHealth;
    private Transform player;
    private NavMeshAgent navAgent;
    private int attackStep = 0;
    private bool isAttacking = false;
    private bool isDead = false;
    private bool isWandering = false;
    private float attackTimer = 0f;
    private Vector3 spawnPoint;

    public GameObject healthBarPrefab;
    public RectTransform healthBarUI;
    public Transform healthBarCanvas;

    private GameManager gameManager;
    private QuestGiver questGiver;

    void Start()
    {
        animationComponent = GetComponent<Animation>();
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        gameManager = FindObjectOfType<GameManager>();
        spawnPoint = transform.position;

        navAgent.speed = walkSpeed;
        navAgent.acceleration = 50f;
        navAgent.stoppingDistance = attackRange - 0.5f;
        navAgent.autoBraking = true;
        navAgent.angularSpeed = 500f;

        currentHealth = maxHitPoints;

        ConfigureAnimations();
        PlayIdleAnimation();
        CreateHealthBar();
        HideHealthBar();

        // Flip the model if needed
        if (modelRoot != null && flipModelUpsideDown)
        {
            Vector3 scale = modelRoot.localScale;
            scale.y *= -1f;
            modelRoot.localScale = scale;
        }

        InvokeRepeating(nameof(Wander), wanderCooldown, wanderCooldown);
        InvokeRepeating(nameof(RegenerateHealth), 1f, 1f);
    }

    public void SetQuestGiver(QuestGiver giver) { questGiver = giver; }

    void ConfigureAnimations()
    {
        if (animationComponent[animations.walk] != null)
            animationComponent[animations.walk].wrapMode = WrapMode.Loop;

        if (animationComponent[animations.idle] != null)
            animationComponent[animations.idle].wrapMode = WrapMode.Loop;

        if (animationComponent[animations.run] != null)
            animationComponent[animations.run].wrapMode = WrapMode.Loop;

        foreach (var attackAnim in animations.attacks)
        {
            if (!string.IsNullOrEmpty(attackAnim) && animationComponent[attackAnim] != null)
                animationComponent[attackAnim].wrapMode = WrapMode.Once;
        }
    }

    void CreateHealthBar()
    {
        healthBarCanvas = GameObject.FindGameObjectWithTag("HealthBarCanvas")?.transform;

        if (healthBarCanvas != null && healthBarPrefab != null)
        {
            GameObject healthBarInstance = Instantiate(healthBarPrefab, healthBarCanvas);
            healthBarUI = healthBarInstance.GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isAttacking)
        {
            if (distanceToPlayer <= attackRange)
            {
                ShowHealthBar();
                StopMoving();
                HandleAttack();
            }
            else if (distanceToPlayer <= detectionRange)
            {
                ShowHealthBar();
                navAgent.isStopped = false;
                navAgent.speed = runSpeed;
                navAgent.SetDestination(player.position);
                PlayRunAnimation();
                isWandering = false;
            }
            else if (!isWandering)
            {
                Wander();
            }
        }

        attackTimer += Time.deltaTime;
        UpdateHealthBarPosition();
        UpdateFacingDirection();
    }

    void Wander()
    {
        if (isDead || isAttacking || isWandering || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange) return;

        isWandering = true;
        Vector3 wanderTarget = GetRandomWanderPoint();
        navAgent.isStopped = false;
        navAgent.SetDestination(wanderTarget);
        PlayWalkAnimation();

        Invoke(nameof(StopMoving), Random.Range(1f, 2f));
    }

    Vector3 GetRandomWanderPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += spawnPoint;
        NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, 1);
        return hit.position;
    }

    void HandleAttack()
    {
        if (attackTimer >= attackCooldown && !isDead)
        {
            attackTimer = 0f;
            attackStep = (attackStep + 1) % Mathf.Min(animations.attacks.Length, 4);

            PlaySwordSwingSound();
            PlayAttack(attackStep);
            DealDamage();
        }
    }

    void DealDamage()
    {
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, attackRadius);
        foreach (Collider hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<PaladinController>()?.TakeDamage(damage);
            }
        }
    }

    void PlayAttack(int step)
    {
        if (isDead || animations.attacks.Length == 0) return;

        isAttacking = true;

        if (step >= 0 && step < animations.attacks.Length && !string.IsNullOrEmpty(animations.attacks[step]))
        {
            animationComponent.Play(animations.attacks[step]);
            PlayRandomSound(attackAudioSource, attackSounds);
            Invoke(nameof(CompleteAttack), animationComponent[animations.attacks[step]].length * 0.9f);
        }
    }

    void CompleteAttack() { isAttacking = false; PlayIdleAnimation(); }

    void PlaySwordSwingSound() { PlayRandomSound(swordAudioSource, swordSwingSounds); }
    void PlaySwordHitSound() { PlayRandomSound(swordAudioSource, swordHitSounds); }

    void PlayRandomSound(AudioSource source, AudioClip[] clips)
    {
        if (source != null && clips.Length > 0)
        {
            source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        ShowHealthBar();
        UpdateHealthUI();

        PlayRandomSound(painAudioSource, painSounds);
        PlaySwordHitSound();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        navAgent.isStopped = true;
        animationComponent.Play(animations.death);
        PlayRandomSound(deathAudioSource, deathSounds);

        gameManager?.RemoveEnemy(gameObject);
        questGiver?.EnemySlain();

        HideHealthBar();
        Destroy(gameObject, 2f);
    }

    void UpdateHealthUI()
    {
        if (healthBarUI != null)
        {
            healthBarUI.localScale = new Vector3(currentHealth / maxHitPoints, 1, 1);
        }
    }

    void UpdateHealthBarPosition()
    {
        if (healthBarUI != null)
        {
            Vector3 worldPosition = transform.position + new Vector3(0, 2f, 0);
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

            if (screenPosition.z > 0)
            {
                healthBarUI.position = screenPosition;
                healthBarUI.sizeDelta = new Vector2(200, 10);
            }
            else
            {
                healthBarUI.gameObject.SetActive(false);
            }
        }
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

    public void BeginCombat()
    {
        isWandering = false;
        isAttacking = false;
        isDead = false;

        if (navAgent != null && player != null)
        {
            navAgent.isStopped = false;
            navAgent.SetDestination(player.position);
            PlayRunAnimation();
        }
    }

    void HideHealthBar() { if (healthBarUI != null) healthBarUI.gameObject.SetActive(false); }
    void ShowHealthBar() { if (healthBarUI != null) healthBarUI.gameObject.SetActive(true); }

    void StopMoving() { isWandering = false; navAgent.isStopped = true; PlayIdleAnimation(); }

    void PlayIdleAnimation() { if (!isDead) animationComponent.Play(animations.idle); }
    void PlayWalkAnimation() { if (!isDead) animationComponent.Play(animations.walk); }
    void PlayRunAnimation() { if (!isDead) animationComponent.Play(animations.run); }

    void UpdateFacingDirection()
    {
        if (isDead || modelRoot == null || navAgent == null) return;

        Vector3 velocity = navAgent.velocity;
        if (velocity.sqrMagnitude > 0.1f)
        {
            Vector3 lookDir = velocity.normalized;
            if (rotateModelBackwards)
                lookDir = -lookDir;

            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
            modelRoot.rotation = Quaternion.Slerp(modelRoot.rotation, targetRot, Time.deltaTime * 10f);
        }
    }
}