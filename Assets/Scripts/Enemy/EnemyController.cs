using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.Events;

public class EnemyController : SharedBehaviourCharacters
{
    [SerializeField] private EffectsLifecycleActivation effectsLifecycleActivation;
    
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private ParticleSystem onMeleeHitPS;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Color easyColor;
    [SerializeField] private Color hardColor;
    
    private float lastMeleeAttackTime;
    private float lastRangedAttackTime;
    private float difficulty;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private MeleeAttackController meleeAttackController;
    private RangedAttackController rangedAttackController;

    public event Action<GameObject> OnEnemyDeath;

    [SerializeField] private UnityEvent uniqueDeathEvents;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        meleeAttackController = GetComponent<MeleeAttackController>();
        rangedAttackController = GetComponent<RangedAttackController>();
        characterAudioManager = GetComponent<CharacterAudioManager>();
        SetInheritedTeam(Team.Enemy);
    }
    
    void Update()
    {
        if (Isdead) return;
        if (PlayerReferenceManager.Instance.playerController.Isdead) return;
        if (PlayerReferenceManager.Instance.PlayerInMenus)
        {
            navMeshAgent.isStopped = true; 
            return;
        }
        RotateTowardsPlayer();

        // Check if the NavMeshAgent is connected and able to move
        if (navMeshAgent == null || !navMeshAgent.isOnNavMesh)
        {
            TryReconnectNavMesh();
        }

        // Ignore the Y-axis and calculate distance only on the XZ plane
        Vector3 flatPlayerPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        float sqrDistanceToPlayer = (flatPlayerPosition - transform.position).sqrMagnitude;

        // Buffer to avoid oscillation between moving and attacking
        float stoppingDistanceSquared = CurrentStats.attackRange * 0.9f * CurrentStats.attackRange * 0.9f; // Add 10% buffer for stopping
        float attackRangeSquared = CurrentStats.attackRange * CurrentStats.attackRange;

        // Check if the enemy is outside the stopping distance (with buffer) before moving
        if (sqrDistanceToPlayer > stoppingDistanceSquared)
        {
            MoveTowardsPlayer(flatPlayerPosition);
        }
        else
        {
            // Stop moving if within the buffer range
            navMeshAgent.isStopped = true;

            // Check if the enemy is within attack range and ready to attack
            if (sqrDistanceToPlayer <= attackRangeSquared)
            {
                if (CurrentStats.attackType == CharacterStats.StatValues.AttackType.Melee && Time.time >= lastMeleeAttackTime + CurrentStats.meleeAttackSpeed)
                {
                    // Melee attack
                    meleeAttackController.PerformMeleeAttack(CurrentStats.boxDimensions, GetTeam(), CurrentStats.attackDamage,characterAudioManager,CurrentStats.attackRange / CurrentStats.DifficultyRanges.attackRangeMax);
                    lastMeleeAttackTime = Time.time;  // Reset melee attack timer
                }
                else if (CurrentStats.attackType == CharacterStats.StatValues.AttackType.Ranged && Time.time >= lastRangedAttackTime + CurrentStats.rangedAttackSpeed)
                {
                    // Ranged attack
                    rangedAttackController.FireProjectile(characterAudioManager);
                    lastRangedAttackTime = Time.time;  // Reset ranged attack timer
                }
            }
        }
    }

    private void MoveTowardsPlayer(Vector3 targetPosition)
    {
        // If the enemy is outside the stopping range (with buffer), continue moving
        if (navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = false;  // Resume movement if it was stopped
            navMeshAgent.SetDestination(targetPosition);  // Move toward the player, only on the XZ plane
        }
    }

    private void TryReconnectNavMesh()
    {
        if (navMeshAgent != null)
        {
            // Check if the agent is enabled but not on the NavMesh
            if (!navMeshAgent.isOnNavMesh)
            {
                // Attempt to reposition the enemy onto the NavMesh
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
                {
                    navMeshAgent.Warp(hit.position);  // Warp the enemy to the nearest valid NavMesh position
                    print("Reconnected NavMeshAgent");
                }
                else
                {
                    print("Failed to reconnect to NavMesh");
                }
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        // Only rotate on the XZ plane (ignore Y-axis)
        Vector3 direction = new Vector3(player.position.x - transform.position.x, 0, player.position.z - transform.position.z).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public void Spawn(float Difficulty)
    {
        // Initialize stats from ScriptableObject
        navMeshAgent.enabled = true;
        SetCharacterStats(CharacterStatsData.GetStatsByNormValue(Difficulty));
        navMeshAgent.speed = CurrentStats.moveSpeed;
        

        meshRenderer.material.color = Color.Lerp(easyColor, hardColor, Difficulty);
        
        ResetCurrentHealth();
        
        player = PlayerReferenceManager.Instance.PlayerTransform;

        lastMeleeAttackTime = -CurrentStats.meleeAttackSpeed; // Use melee attack speed for cooldown
        lastRangedAttackTime = -CurrentStats.rangedAttackSpeed; // Use ranged attack speed for cooldown

        // Set up attack type based on the enemy stats
        if (CurrentStats.attackType == CharacterStats.StatValues.AttackType.Melee)
        {
            meleeAttackController.enabled = true;
            rangedAttackController.enabled = false;
        }
        else
        {
            meleeAttackController.enabled = false;
            rangedAttackController.enabled = true;
        }
        
        effectsLifecycleActivation.StartActivate();
        SetIsDead(false);
        
        characterAudioManager.PlaySpawnAudio();


    }

    public override void TakeDamage(int amount, GameObject attacker, bool isRetaliatory)
    {
        base.TakeDamage(amount,attacker,isRetaliatory);
        

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        if (Isdead) return;
        navMeshAgent.enabled = false;
        SetIsDead(true);


        DifficultyManager.Instance.ProcessKillResponse(true);
        int xpReward = (int)Mathf.Lerp(5, CurrentStats.xpReward, DifficultyManager.Instance.GetCurrentDifficulty());
        
        GameDataManager.Instance.AddKill();
        
        XPManager.Instance.AddXP(xpReward,true);
        ScoreManager.Instance.AddScore(difficulty,true);
        characterAudioManager.PlayDeathAudio();
        
        uniqueDeathEvents.Invoke();
        effectsLifecycleActivation.EndDeactivate();
        OnEnemyDeath?.Invoke(gameObject);

    }
    
    
}
