using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingProjectileManager : SpecialAbilityBehavior
{
    [SerializeField] private string projectileTag;
    [SerializeField] private float orbitRadius = 1.5f;
    private float orbitSpeed = 250f;

    private List<Projectile> floatingProjectiles = new List<Projectile>();
    private SharedBehaviourCharacters sharedBehaviourCharacters;
    private Team team;
    private int maxQuantity;
    private float spawnTimer;
    private string abilityName = "FloatingProjectileManager";
    private bool isSpawning = false;

    private void Start()
    {
        sharedBehaviourCharacters = GetComponent<SharedBehaviourCharacters>();
        team = sharedBehaviourCharacters.Team;

        if (specialAbility == null)
        {
            specialAbility = AbilitySingleton.Instance.GetAbilityData(abilityName);
        }

        projectileTag = GetComponent<PlayerController>() != null ? "Projectile_Player" : "Projectile_Enemy";
        maxQuantity = specialAbility.abilityStats.quantity;
        spawnTimer = specialAbility.abilityStats.cooldown;
    }

    private void Update()
    {
        if (!PlayerReferenceManager.Instance.PlayerInMenus && !sharedBehaviourCharacters.Isdead)
        {
            UpdateCooldown(Time.deltaTime);
            UpdateProjectilePositions();

            // Check if spawning and projectile count is managed properly
            if (!isSpawning && floatingProjectiles.Count < maxQuantity)
            {
                StartCoroutine(SpawnProjectileWithDelay());
            }
        }
    }

    private IEnumerator SpawnProjectileWithDelay()
    {
        isSpawning = true; // Set spawning flag

        yield return new WaitForSeconds(spawnTimer);

        if (floatingProjectiles.Count < maxQuantity) // Only spawn if under max limit
        {
            SpawnProjectile();
            //Debug.Log("Spawned a new projectile. Total active projectiles: " + floatingProjectiles.Count);
        }

        isSpawning = false; // Reset flag after spawning
    }

    private void UpdateProjectilePositions()
    {
        float angleStep = 360f / Mathf.Max(floatingProjectiles.Count, 1);
        for (int i = 0; i < floatingProjectiles.Count; i++)
        {
            float angle = angleStep * i + (Time.time * orbitSpeed) % 360f;
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * orbitRadius;
            floatingProjectiles[i].transform.position = transform.position + offset;
        }
    }

    private void SpawnProjectile()
    {
        GameObject projectileObject = MultiObjectPooler.Instance.SpawnFromPool(projectileTag, transform.position, Quaternion.identity);
        if (projectileObject != null)
        {
            Projectile projectile = projectileObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Spawn(team, sharedBehaviourCharacters.CurrentStats.attackDamage, Mathf.Infinity, sharedBehaviourCharacters);
                floatingProjectiles.Add(projectile);
                projectile.OnProjectileTriggered += HandleProjectileTriggered;
            }
        }
        else
        {
            Debug.LogWarning("Projectile spawn failed, pool is empty or tag is incorrect.");
        }
    }

    private void HandleProjectileTriggered(Projectile projectile)
    {
        if (floatingProjectiles.Contains(projectile))
        {
            floatingProjectiles.Remove(projectile);
            projectile.OnProjectileTriggered -= HandleProjectileTriggered;
            projectile.ReturnToPool();
            Debug.Log("Projectile removed. Total active projectiles: " + floatingProjectiles.Count);
        }
    }

    public override void Activate()
    {
        if (floatingProjectiles.Count < maxQuantity && !isSpawning)
        {
            SpawnProjectile();
        }
    }
}
