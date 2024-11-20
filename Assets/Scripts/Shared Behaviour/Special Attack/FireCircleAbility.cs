using System;
using UnityEngine;

public class FireCircleAbility : SpecialAbilityBehavior
{
    private SharedBehaviourCharacters sharedBehaviourCharacters;
    [SerializeField] private string projectileTag;
    private string abilityName = "FireCircleAbility";
    private Team team;

    private float radius = 3f;  // Radius around the player where projectiles will spawn

    private void Update()
    {
        if (!PlayerReferenceManager.Instance.PlayerInMenus && !sharedBehaviourCharacters.Isdead)
        {
            UpdateCooldown(Time.deltaTime);
        }
    }

    public void Start()
    {
        sharedBehaviourCharacters = GetComponent<SharedBehaviourCharacters>();
        team = sharedBehaviourCharacters.Team;

        if (specialAbility == null)
        {
            specialAbility = AbilitySingleton.Instance.GetAbilityData(abilityName);
        }
        
        if (GetComponent<PlayerController>() != null)
        {
            projectileTag = "Projectile_Player";
            radius = 1;
        }
        else
        {
            projectileTag = "Projectile_Enemy";
            radius = 1.5f;
        }


    }

    public override void Activate()
    {
        // Calculate the forward-facing direction for the fire projectiles
        Vector3 forwardDirection = transform.forward;

        // Instantiate fire projectiles in a circle around the character, relative to forward direction
        for (int i = 0; i < specialAbility.abilityStats.quantity; i++)
        {
            float angle = i * (360f / specialAbility.abilityStats.quantity);  // Spread fire evenly in a circle
            Vector3 fireDirection = Quaternion.Euler(0, angle, 0) * forwardDirection;

            Vector3 spawnPosition = transform.position + fireDirection * radius;

            GameObject fireProjectile = MultiObjectPooler.Instance.SpawnFromPool(projectileTag, spawnPosition, Quaternion.LookRotation(fireDirection));

            if (fireProjectile != null)
            {
                Projectile projectile = fireProjectile.GetComponent<Projectile>();
                if (projectile != null)
                {
                    projectile.SetDirection(fireDirection);
                    projectile.Spawn(team, sharedBehaviourCharacters.CurrentStats.attackDamage, sharedBehaviourCharacters.CurrentStats.attackRange,sharedBehaviourCharacters);
                    sharedBehaviourCharacters.characterAudioManager.PlayOnceFromCollection(specialAbility.specialAudioCollection);
                }
            }
        }
    }
}