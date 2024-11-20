using System;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PlayerData
{

    public enum SkillTypes { Health = 0, AttackDamage = 1, MoveSpeed = 2, MeleeSpeed = 3, RangedSpeed = 4, AttackRange = 5 } 
    [Header("Player Pips")]
    public int healthPips;
    public int attackDamagePips;
    public int moveSpeedPips;
    public int meleeAttackSpeedPips;
    public int rangedAttackSpeedPips;
    public int attackRangePips;
    
    [Header("Player Data")]
    public int spentPips;
    public int availablePoints;
    public int level;
    public int currentXp;
    public int chroniclesCompleted;

    public int baseCost = 5;

    public PlayerData DeepCopy()
    {
        return (PlayerData)this.MemberwiseClone();
    }
    
    public int CalculateCostStatBuy()
    {
        return (spentPips + 1) * baseCost;  // The cost should increase with each spent pip
    }

    public bool CanAffordToBuy()
    {
        return (availablePoints >= CalculateCostStatBuy());
    }
    
    public int CalculateSpentAvailablePoints()
    {
        int totalPipCost = 0;
    
        // Sum the cost for all spent pips
        for (int i = 1; i <= spentPips; i++)
        {
            totalPipCost += i * baseCost;  // Each pip costs more than the previous one
        }

        // Calculate available points based on level and total spent pips
        return availablePoints = Mathf.Clamp(
            (XPManager.Instance.GetCurrentLevel() * baseCost) - totalPipCost, 
            0, 10000
        );
    }

    public int GetPipOfSkill(SkillTypes skilltypes)
    {
        return skilltypes switch
        {
            SkillTypes.Health => healthPips,
            SkillTypes.AttackDamage => attackDamagePips,
            SkillTypes.MoveSpeed => moveSpeedPips,
            SkillTypes.MeleeSpeed => meleeAttackSpeedPips,
            SkillTypes.RangedSpeed => rangedAttackSpeedPips,
            SkillTypes.AttackRange => attackRangePips,
            _ => throw new ArgumentOutOfRangeException(nameof(skilltypes), skilltypes, null)
        };
    }

}