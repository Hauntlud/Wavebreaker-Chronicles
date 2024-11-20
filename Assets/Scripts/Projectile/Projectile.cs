using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private EffectsLifecycleActivation effectsLifecycleActivation;
    [SerializeField] private SharedBehaviourCharacters sharedBehaviourCharacters;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private bool isActive = true;
    public event Action<Projectile> OnProjectileTriggered;  // Event triggered on hit for floating projectiles

    public float speed = 20f;
    public int damage = 10;
    public float lifetime = 5f;  // How long the projectile lasts before returning to the pool
    private Vector3 moveDirection;  // Direction to move the projectile
    private float lifetimeTimer;
    private Team thisTeam;
    private bool isPaused = false;  // Track if the projectile is paused

    private void Reset()
    {
        effectsLifecycleActivation = GetComponent<EffectsLifecycleActivation>();
    }

    void Update()
    {
        // Pause/resume the projectile based on the player's menu status
        if (PlayerReferenceManager.Instance.PlayerInMenus && !isPaused)
        {
            isPaused = true;
            return;
        }
        else if (!PlayerReferenceManager.Instance.PlayerInMenus && isPaused)
        {
            isPaused = false;
        }

        if (!isActive || isPaused) return;  // Do nothing if inactive or paused

        // Move the projectile forward in world space based on the moveDirection and speed
        transform.Translate(moveDirection * (speed * Time.deltaTime), Space.World);

        // Lifetime check
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= lifetime)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SharedBehaviourCharacters target = other.GetComponent<SharedBehaviourCharacters>();
        if (target != null && target.GetTeam() != thisTeam)
        {
            target.TakeDamage(damage, sharedBehaviourCharacters.gameObject, sharedBehaviourCharacters.AutoRetaliateOn);
            ReturnToPool();
            OnProjectileTriggered?.Invoke(this);
        }
    }

    public void ReturnToPool()
    {
        if (!isActive) return;  // If it's already returned to the pool, don't return again

        isActive = false;  // Mark the projectile as inactive
        moveDirection = Vector3.zero;  // Reset direction to stop movement
        effectsLifecycleActivation.EndDeactivate();  // Handle visual effects (if any)
    }

    // Spawn the projectile with the appropriate stats
    public void Spawn(Team team, int damageIn, float lifetimeIn, SharedBehaviourCharacters sharedBehaviourCharactersIn)
    {
        sharedBehaviourCharacters = sharedBehaviourCharactersIn;
        isActive = true;
        isPaused = false;
        lifetime = lifetimeIn;
        thisTeam = team;
        lifetimeTimer = 0f;
        damage = damageIn;

        print(lifetimeIn);
        effectsLifecycleActivation.StartActivate();
    }

    // Set the projectile's movement direction
    public void SetDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;  // Normalize the direction to ensure it’s valid
    }
}
