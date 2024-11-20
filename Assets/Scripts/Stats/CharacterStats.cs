using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "Stats/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    // Inner class to encapsulate all character-related data
    [System.Serializable]
    public class StatValues
    {
        // Common Stats for both Player and Enemy
        public int xpReward = 25;
        public int maxHealth = 100;
        public float moveSpeed = 5f;
        public int attackDamage = 20;
        public float meleeAttackSpeed = 1f;  // Melee attack speed
        public float rangedAttackSpeed = 1f;  // Ranged attack speed
        public Vector3 boxDimensions = new Vector3(1f, 1f, 1f);
        public float attackRange;

        public DifficultyRanges DifficultyRanges = new DifficultyRanges();
        
        // Unique properties for enemies or players
        public enum AttackType { Melee, Ranged }
        public AttackType attackType;

        [FormerlySerializedAs("hasAOEMelee")]
        public bool hasAOE;
        

        // Method to make a deep copy of the StatValues class
        public StatValues DeepCopy()
        {
            return (StatValues)this.MemberwiseClone();
        }
    }
    
    [System.Serializable]
    public class DifficultyRanges
    {
        // Ranges for adaptive difficulty
        [Header("Health")] 
        public int minHealth = 50;
        public int maxHealth = 150;
        
        [Header("Move Speed")] 
        public float minMoveSpeed = 3f;
        public float maxMoveSpeed = 10f;

        [Header("Attack Damage")] 
        public int minAttackDamage = 10;
        public int maxAttackDamage = 40;

        [Header("Attack Melee Cooldown")] 
        public float minAttackMeleeCooldown = 0.3f;
        public float maxAttackMeleeCooldown = 1.5f;
        
        [Header("Attack Range Cooldown")] 
        public float minAttackRangeCooldown = 0.3f;
        public float maxAttackRangeCooldown = 1.5f;

        [Header("Attack Range")] 
        public float attackRangeMin = 1.5f;
        public float attackRangeMax = 3f;
        
        [Header("Box Dimensions")] 
        public Vector3 boxDimensionsMin = new Vector3(1.5f, 1f, 1.5f);
        public Vector3 boxDimensionsMax = new Vector3(3f, 1f, 3f);
            
        public DifficultyRanges DeepCopy()
        {
            return (DifficultyRanges)this.MemberwiseClone();
        }
    }

    [SerializeField] private StatValues baseStats = new StatValues();

    // Public method to get the base stats
    public StatValues GetBaseStats()
    {
        // Return a deep copy to ensure the original stats are not altered
        return baseStats.DeepCopy();
    }
    
    public StatValues SetStats(StatValues statValuesModified,StatValues statValuesOriginal)
    {
        statValuesOriginal = statValuesModified.DeepCopy();
        return statValuesOriginal;
    }
    
    public CharacterStats.StatValues GetStatsByNormValue(float normValue)
    {
        // Get a deep copy of the base stats to modify and return
        StatValues newStats = baseStats.DeepCopy();

        // Lerp each stat between its minimum and maximum values based on the normalized value
        newStats.maxHealth = (int)Mathf.Lerp(newStats.DifficultyRanges.minHealth, newStats.DifficultyRanges.maxHealth, normValue);
        newStats.moveSpeed = Mathf.Lerp(newStats.DifficultyRanges.minMoveSpeed, newStats.DifficultyRanges.maxMoveSpeed, normValue);
        newStats.attackDamage = (int)Mathf.Lerp(newStats.DifficultyRanges.minAttackDamage, newStats.DifficultyRanges.maxAttackDamage, normValue);
        newStats.meleeAttackSpeed = Mathf.Lerp(newStats.DifficultyRanges.maxAttackMeleeCooldown, newStats.DifficultyRanges.minAttackMeleeCooldown, normValue);
        newStats.rangedAttackSpeed = Mathf.Lerp(newStats.DifficultyRanges.maxAttackRangeCooldown, newStats.DifficultyRanges.minAttackRangeCooldown, normValue);
        newStats.attackRange = Mathf.Lerp(newStats.DifficultyRanges.attackRangeMin, newStats.DifficultyRanges.attackRangeMax, normValue);
        newStats.boxDimensions = Vector3.Lerp(newStats.DifficultyRanges.boxDimensionsMin, newStats.DifficultyRanges.boxDimensionsMax, normValue);

        // Return the modified deep copy of the stats
        return newStats;
    }
}