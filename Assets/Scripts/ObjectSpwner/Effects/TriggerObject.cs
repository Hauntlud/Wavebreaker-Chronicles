using System;
using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    [SerializeField] private GeneralEffect effect;  // Reference to the ScriptableObject holding effect data
    public event Action<GameObject> OnInteracted;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Ensure the object interacting is the player
        {
            // Apply the effect to the player based on the effect type
            ApplyEffectToPlayer(other.gameObject, effect);

            // Optionally, destroy the object or disable it after interaction
            Interact();
        }
    }
    
    public void Interact()
    {
        OnInteracted?.Invoke(gameObject);
    }

    private void ApplyEffectToPlayer(GameObject player, GeneralEffect effect)
    {
        switch (effect.effectType)
        {
            case EffectType.AttackSpeed:
                // Apply attack speed effect to player
                //Debug.Log($"Increased attack speed by {effect.effectValue} for {effect.duration} seconds");
                // You will implement the actual logic here, such as modifying the player's stats
                break;

            case EffectType.MoveSpeed:
                // Apply move speed effect to player
                PlayerReferenceManager.Instance.playerController.ApplyMoveSpeedBoost(effect.duration,effect.effectValue);
                PlayerReferenceManager.Instance.playerController.characterAudioManager.PlayPickUpSpeedSound();
                //Debug.Log($"Increased move speed by {effect.effectValue} for {effect.duration} seconds");
                // Implement the actual logic to change player's move speed
                break;

            case EffectType.Health:
                // Apply health effect to player
                PlayerReferenceManager.Instance.playerController.AddPlayerHealth((int)effect.effectValue);
                //Debug.Log($"Increased health by {effect.effectValue}");
                // Implement health logic, such as increasing health points
                break;

            // Add more cases for other effect types as needed
            case EffectType.XP:
                PlayerReferenceManager.Instance.playerController.characterAudioManager.PlayPickUpXPSound();
                XPManager.Instance.AddXP((int)effect.effectValue);
                break;

            default:
                Debug.LogWarning("Unknown effect type!");
                break;
        }
    }
}