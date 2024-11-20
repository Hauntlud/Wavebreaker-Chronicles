using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewSpecialAbility", menuName = "Abilities/SpecialAbility")]
public class SpecialAbilityData : ScriptableObject
{
    public string abilityName;   // Name of the ability
    public string abilityNameString;   // Name of the ability
    public GameObject abilityPrefab;  // Prefab used for the ability (if any)
    public GameObject particleEffectPlayer;  // Prefab used for the ability (if any)
    public GameObject particleEffectEnemy;  // Prefab used for the ability (if any)
    public AudioCollection specialAudioCollection;  // Prefab used for the ability (if any)
    public Sprite spriteImage;  // Prefab used for the ability (if any)
    public Material playerMaterial;  // Prefab used for the ability (if any)
    public Material enemymaterial;  // Prefab used for the ability (if any)
    public AbilityLevelStats abilityStats; // List of stats per level
}

[System.Serializable]
public class AbilityLevelStats
{
    public float cooldown;
    public int quantity;
    public float speed;
    public float damage;
    public float range;
}