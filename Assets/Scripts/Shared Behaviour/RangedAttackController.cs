using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class RangedAttackController : MonoBehaviour
{
    public Transform firePoint;
    public string projectileTag;
    
    [SerializeField] private ParticleSystem fireEffect;
    [SerializeField] private SharedBehaviourCharacters sharedBehaviourCharacters;
    [SerializeField] private float lifeTime = 5;
    
    public UnityEvent triggeredWhenFired;

    private void Start()
    {
        sharedBehaviourCharacters = GetComponent<SharedBehaviourCharacters>();
    }

    public void FireProjectile(CharacterAudioManager characterAudioManager)
    {
        characterAudioManager.PlayRangeAttackAudio();
        fireEffect.Play();
    
        // Get the projectile from the pool
        GameObject projectile = MultiObjectPooler.Instance.SpawnFromPool(projectileTag, firePoint.position, firePoint.rotation);

        if (projectile != null)
        {
            // Get the projectile script
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            if (projectileScript != null)
            {
                projectileScript.SetDirection(transform.forward);  // Use character's forward instead of firePoint
                
                // Set the projectile stats (team, damage, lifetime)
                projectileScript.Spawn(sharedBehaviourCharacters.GetTeam(), sharedBehaviourCharacters.GetStats().attackDamage / 2, sharedBehaviourCharacters.GetStats().attackRange/4,sharedBehaviourCharacters);


            }
        }

        triggeredWhenFired.Invoke();
    }

    
    [Button("Find AttackPoint")]
    public void FindAttackPoint()
    {
        var children = GetComponentsInChildren<Transform>();

        foreach (var child in children)
        {
            if (child.transform.CompareTag("AttackPoint"))
            {
                firePoint = child.transform;
            }
        }
    }

    public void SetDestroyRange(float range)
    {
        lifeTime = range/2;
    }
}