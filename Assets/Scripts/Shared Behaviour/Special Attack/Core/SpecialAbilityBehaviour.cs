using UnityEngine;

public abstract class SpecialAbilityBehavior : MonoBehaviour
{
    public SpecialAbilityData specialAbility;  // This will reference the encapsulated SpecialAbility class
    private float cooldownTimer;

    // Call this to update the cooldown and activate the ability when ready
    public void UpdateCooldown(float deltaTime)
    {
        cooldownTimer -= deltaTime;
        if (cooldownTimer <= 0)
        {
            Activate();
            cooldownTimer = specialAbility.abilityStats.cooldown;  // Reset the timer using upgraded cooldown
        }
    }

    // This method should be overridden in each unique ability to define its behavior
    public abstract void Activate();
    

    
}