using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStatSystem : MonoBehaviour
{
    [SerializeField] private CharacterStats.StatValues currentStats;  // Current stats being modified
    [SerializeField] private PlayerData liveGamePlayerData;  // Temporary stat changes made through UI before locking in
    [SerializeField] private PlayerData statBuyingPlayerData;  // Temporary stat changes made through UI before locking in
    
    public PlayerData LiveGamePlayerData => liveGamePlayerData;
    public PlayerData StatBuyingPlayerData => statBuyingPlayerData;
    public CharacterStats.StatValues CurrentStats => currentStats;
    
    private const int maxPips = 10;  // Max pips for each skill
    private const int baseCost = 5;  // Base cost per pip


    public void LoadNewLivePlayerData()
    {
        liveGamePlayerData = new PlayerData();
        if (GameSaveManager.Instance.LoadPlayerData(out liveGamePlayerData))
        {
            //Debug.Log("Player data loaded successfully.");
            // Use loaded player data in your game
        }
        else
        {
            Debug.LogWarning("No player data found. Creating new data.");
            liveGamePlayerData = new PlayerData(); // Create default player data
        }

        statBuyingPlayerData = liveGamePlayerData.DeepCopy();
    }

    public void SavePlayerData()
    {
        GameSaveManager.Instance.SavePlayerData(LiveGamePlayerData);
    }
    
    
    public bool EnoughToBuy()
    {
        return liveGamePlayerData.CanAffordToBuy();
    }
    
    public void UpdateAvailablePoints()
    {
        liveGamePlayerData.CalculateSpentAvailablePoints();
        statBuyingPlayerData.CalculateSpentAvailablePoints();
    }

    public void StartPointBuy()
    {
        ResetSkillUI();
        UpdateAvailablePoints();
    }
    

    private void ResetSkillUI()
    {
        statBuyingPlayerData = liveGamePlayerData.DeepCopy();
    }

    public void SetCurrentSkillPoints()
    {
        liveGamePlayerData = statBuyingPlayerData.DeepCopy();
        UpdateStatsFromData();
        UpdateAvailablePoints();
    }
    
    public void SpendPoints(PlayerData.SkillTypes skillType)
    {
        if (!StatBuyingPlayerData.CanAffordToBuy()) return;
        
        switch (skillType)
        {
            case PlayerData.SkillTypes.Health:
                statBuyingPlayerData.healthPips++;
                break;
            case PlayerData.SkillTypes.AttackDamage:
                statBuyingPlayerData.attackDamagePips++;
                break;
            case PlayerData.SkillTypes.MoveSpeed:
                statBuyingPlayerData.moveSpeedPips++;
                break;
            case PlayerData.SkillTypes.MeleeSpeed:
                statBuyingPlayerData.meleeAttackSpeedPips++;
                break;
            case PlayerData.SkillTypes.RangedSpeed:
                statBuyingPlayerData.rangedAttackSpeedPips++;
                break;
            case PlayerData.SkillTypes.AttackRange:
                statBuyingPlayerData.attackRangePips++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(skillType), skillType, null);
        }

        statBuyingPlayerData.spentPips++;
        statBuyingPlayerData.CalculateSpentAvailablePoints();
        SkillUIManager.Instance.UpdateAvailablePointsUI();
    }
    
    public void SubtractPoints(PlayerData.SkillTypes skillType)
    {
        
        switch (skillType)
        {
            case PlayerData.SkillTypes.Health:
                statBuyingPlayerData.healthPips--;
                break;
            case PlayerData.SkillTypes.AttackDamage:
                statBuyingPlayerData.attackDamagePips--;
                break;
            case PlayerData.SkillTypes.MoveSpeed:
                statBuyingPlayerData.moveSpeedPips--;
                break;
            case PlayerData.SkillTypes.MeleeSpeed:
                statBuyingPlayerData.meleeAttackSpeedPips--;
                break;
            case PlayerData.SkillTypes.RangedSpeed:
                statBuyingPlayerData.rangedAttackSpeedPips--;
                break;
            case PlayerData.SkillTypes.AttackRange:
                statBuyingPlayerData.attackRangePips--;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(skillType), skillType, null);
        }
        
        statBuyingPlayerData.spentPips--;
        statBuyingPlayerData.CalculateSpentAvailablePoints();
    }
    
    private void UpdateStatsFromData()
    {
        var refToCharacterStats = PlayerReferenceManager.Instance.playerController.CharacterStatsData.GetBaseStats();
        // Update current stats based on the provided data
        currentStats.maxHealth = (int)Mathf.Lerp(refToCharacterStats.DifficultyRanges.minHealth, refToCharacterStats.DifficultyRanges.maxHealth, statBuyingPlayerData.healthPips / (float)maxPips);
        currentStats.attackDamage = (int)Mathf.Lerp(refToCharacterStats.DifficultyRanges.minAttackDamage, refToCharacterStats.DifficultyRanges.maxAttackDamage, statBuyingPlayerData.attackDamagePips / (float)maxPips);
        currentStats.moveSpeed = Mathf.Lerp(refToCharacterStats.DifficultyRanges.minMoveSpeed, refToCharacterStats.DifficultyRanges.maxMoveSpeed, statBuyingPlayerData.moveSpeedPips / (float)maxPips);
        currentStats.meleeAttackSpeed = Mathf.Lerp(refToCharacterStats.DifficultyRanges.maxAttackMeleeCooldown, refToCharacterStats.DifficultyRanges.minAttackMeleeCooldown, statBuyingPlayerData.meleeAttackSpeedPips / (float)maxPips);
        currentStats.rangedAttackSpeed = Mathf.Lerp(refToCharacterStats.DifficultyRanges.maxAttackRangeCooldown, refToCharacterStats.DifficultyRanges.minAttackRangeCooldown, statBuyingPlayerData.rangedAttackSpeedPips / (float)maxPips);
        currentStats.attackRange = Mathf.Lerp(refToCharacterStats.DifficultyRanges.attackRangeMin, refToCharacterStats.DifficultyRanges.attackRangeMax, statBuyingPlayerData.attackRangePips / (float)maxPips);
        currentStats.boxDimensions = Vector3.Lerp(refToCharacterStats.DifficultyRanges.boxDimensionsMin, refToCharacterStats.DifficultyRanges.boxDimensionsMax, statBuyingPlayerData.attackRangePips / (float)maxPips);
        
        PlayerReferenceManager.Instance.playerController.UpdateStats(currentStats);
        
    }
    
}
