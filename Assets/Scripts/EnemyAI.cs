using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, ITakeDamage
{
    const string RunTrigger = "Run";
    const string CrouchTrigger = "Crouch";
    const string ShootTrigger = "Shoot";

    [SerializeField] float startingHealth = 100f;
    [SerializeField] float minTimeUnderCover = 0.5f;
    [SerializeField] float maxTimeUnderCover = 2f;
    [SerializeField] int minShotsToTake = 2;
    [SerializeField] int maxShotsToTake = 4;
    [SerializeField] float timeBetweenShots = 0.4f;
    [SerializeField] float rotationSpeed = 120f;
    [SerializeField] float damage = 1f;
    [Range(0, 100)]
    [SerializeField] float shootingAccuracy = 70f;
    [SerializeField] int scoreValue = 5;
    [SerializeField] float coverArrivalDistance = 1.5f;
    [SerializeField] float nearCoverStuckTime = 0.4f;
    [SerializeField] float maxTravelTime = 20f;
    [Range(0, 100)]
    [SerializeField] float crouchChance = 50f;

    [SerializeField] Transform shootingPosition;
    [SerializeField] ParticleSystem bloodSplatterFX;

    bool isShooting;
    bool shootingStarted;
    bool isDead;
    bool useCrouchStance;
    Coroutine shootingCycleRoutine;

    NavMeshAgent agent;
    Player player;
    Transform occupiedCoverSpot;
    Animator animator;

    public Transform CoverSpot => occupiedCoverSpot;

    float travelTimer;
    float nearCoverTimer;

    float _health;
    public float health
    {
        get => _health;
        set => _health = Mathf.Clamp(value, 0, startingHealth);
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        _health = startingHealth;
    }

    public void Init(Player targetPlayer, Transform coverSpot)
    {
        occupiedCoverSpot = coverSpot;
        player = targetPlayer;
        useCrouchStance = Random.Range(0, 100) < crouchChance;
        travelTimer = 0f;
        nearCoverTimer = 0f;
        GetToCover();
    }

    void Start()
    {
        if (player != null || occupiedCoverSpot != null)
        {
            return;
        }

        player = FindObjectOfType<Player>();
        if (player == null)
        {
            return;
        }

        StopMovingAndBeginShooting();
    }

    void GetToCover()
    {
        if (occupiedCoverSpot == null)
        {
            StopMovingAndBeginShooting();
            return;
        }

        Vector3 destination = occupiedCoverSpot.position;
        if (!NavMesh.SamplePosition(destination, out NavMeshHit hit, 3f, NavMesh.AllAreas))
        {
            StopMovingAndBeginShooting();
            return;
        }

        destination = hit.position;

        agent.isStopped = false;
        agent.stoppingDistance = 0.3f;
        agent.SetDestination(destination);

        if (animator != null)
        {
            animator.SetTrigger(RunTrigger);
        }
    }

    void Update()
    {
        if (isDead || shootingStarted)
        {
            if (isShooting && player != null)
            {
                RotateTowardsPlayer();
            }

            return;
        }

        if (occupiedCoverSpot == null)
        {
            return;
        }

        travelTimer += Time.deltaTime;

        if (HasReachedCover() || travelTimer >= maxTravelTime)
        {
            StopMovingAndBeginShooting();
        }
    }

    bool HasReachedCover()
    {
        float distanceToCoverSpot = GetHorizontalDistance(occupiedCoverSpot.position);

        if (distanceToCoverSpot <= coverArrivalDistance)
        {
            return true;
        }

        if (distanceToCoverSpot <= coverArrivalDistance + 1f)
        {
            if (agent.velocity.sqrMagnitude < 0.02f)
            {
                nearCoverTimer += Time.deltaTime;
            }
            else
            {
                nearCoverTimer = 0f;
            }

            if (nearCoverTimer >= nearCoverStuckTime)
            {
                return true;
            }
        }
        else
        {
            nearCoverTimer = 0f;
        }

        if (agent.pathPending || !agent.hasPath)
        {
            return false;
        }

        if (!float.IsInfinity(agent.remainingDistance)
            && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            return true;
        }

        return false;
    }

    float GetHorizontalDistance(Vector3 target)
    {
        Vector3 offset = target - transform.position;
        offset.y = 0f;
        return offset.magnitude;
    }

    void StopMovingAndBeginShooting()
    {
        agent.isStopped = true;
        agent.ResetPath();

        if (animator != null)
        {
            animator.ResetTrigger(RunTrigger);
            if (useCrouchStance)
            {
                animator.SetTrigger(CrouchTrigger);
            }
        }

        BeginShootingCycle();
    }

    void BeginShootingCycle()
    {
        if (shootingStarted || isDead)
        {
            return;
        }

        shootingStarted = true;
        shootingCycleRoutine = StartCoroutine(ShootingCycleCO());
    }

    IEnumerator ShootingCycleCO()
    {
        while (!isDead)
        {
            if (player == null)
            {
                player = FindObjectOfType<Player>();
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            if (player.IsDead)
            {
                yield break;
            }

            PrepareForNextBurst();
            yield return new WaitForSeconds(Random.Range(minTimeUnderCover, maxTimeUnderCover));

            isShooting = true;
            int shotsToFire = Random.Range(minShotsToTake, maxShotsToTake + 1);

            for (int i = 0; i < shotsToFire; i++)
            {
                if (isDead || player == null || player.IsDead)
                {
                    yield break;
                }

                RotateTowardsPlayer();

                if (animator != null)
                {
                    animator.SetTrigger(ShootTrigger);
                }

                yield return new WaitForSeconds(0.35f);
                PerformShot();
                yield return new WaitForSeconds(timeBetweenShots);
            }

            isShooting = false;
        }
    }

    void PrepareForNextBurst()
    {
        if (animator == null || !useCrouchStance)
        {
            return;
        }

        animator.SetTrigger(CrouchTrigger);
    }

    public void Shoot()
    {
        PerformShot();
    }

    void PerformShot()
    {
        if (isDead || player == null || player.IsDead)
        {
            return;
        }

        if (Random.Range(0, 100) >= shootingAccuracy)
        {
            return;
        }

        TryDamagePlayer();
    }

    void TryDamagePlayer()
    {
        if (shootingPosition == null)
        {
            player.TakeDamage(damage);
            return;
        }

        Vector3 direction = player.GetHeadPosition() - shootingPosition.position;
        if (Physics.Raycast(shootingPosition.position, direction, out RaycastHit hit))
        {
            Player hitPlayer = hit.collider.GetComponentInParent<Player>();
            if (hitPlayer != null)
            {
                hitPlayer.TakeDamage(damage);
                return;
            }
        }

        player.TakeDamage(damage);
    }

    void RotateTowardsPlayer()
    {
        Vector3 direction = player.GetHeadPosition() - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            rotation,
            rotationSpeed * Time.deltaTime);
    }

    public void TakeDamage(Weapon weapon, Projectile projectile, Vector3 contactPoint)
    {
        if (isDead)
        {
            return;
        }

        health -= weapon.GetDamage();

        if (bloodSplatterFX != null)
        {
            ParticleSystem effect = Instantiate(
                bloodSplatterFX,
                contactPoint,
                Quaternion.LookRotation(weapon.transform.position - contactPoint));
            effect.Stop();
            effect.Play();
        }

        if (health <= 0f)
        {
            isDead = true;

            if (shootingCycleRoutine != null)
            {
                StopCoroutine(shootingCycleRoutine);
            }

            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(scoreValue);
                ScoreManager.instance.PlayEnemyDefeatSound();
            }

            Destroy(gameObject);
        }
    }
}
