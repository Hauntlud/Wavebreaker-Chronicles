using UnityEngine;
using System.Collections;
using NaughtyAttributes;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }  // Singleton instance

    [SerializeField] private AnimationCurve xpCurve;  // Animation curve to control XP scaling
    [SerializeField] private int baseXP = 1000;  // Base XP required for first level
    [SerializeField] private int maxLevelScalingPoint = 100;  // Point where scaling stops, XP required stays constant

    [SerializeField] private int currentLevel = 1;  // Player's current level
    [SerializeField] private int currentXP = 0;  // Player's current XP
    [SerializeField] private int xpToNextLevel;  // XP required to reach the next level

    [SerializeField] private float xpMultiplier = 1f;  // XP multiplier that increases with kills
    [SerializeField] private float multiplierIncrement = 0.1f;  // How much the multiplier increases per kill
    [SerializeField] private float maxMultiplier = 3f;  // Maximum multiplier limit
    [SerializeField] private float multiplierResetCooldown = 3f;  // Time before the multiplier resets
    private Coroutine resetMultiplierCoroutine;


    [SerializeField] private int killedDuringMulti;
    [SerializeField] private int maxKilledDuringMulti;
    public float XpMultiplier => xpMultiplier;
    public int MaxKilledDuringMulti => maxKilledDuringMulti;

    private void Awake()
    {
        // Ensure the singleton is set
        transform.parent = null;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    public void Initialize()
    {
        // Initialize XP to the next level based on the curve

        xpToNextLevel = GetXPForNextLevel();
        GameSaveManager.Instance.LoadXPData();
    }

    private int GetXPForNextLevel()
    {
        if (currentLevel >= maxLevelScalingPoint)
        {
            // After maxLevelScalingPoint, XP required stays constant
            return baseXP;
        }

        // Use the animation curve to calculate the XP needed for the next level
        float curveValue = xpCurve.Evaluate((float)currentLevel / maxLevelScalingPoint);
        return Mathf.CeilToInt(curveValue * baseXP);
    }

    [Button("Add 1000 xp")]
    public void AddXPInspector()
    {
        AddXP(1000, true);
    }
    
    public void AddXP(int amount, bool isKill = true)
    {
        if (isKill) OnKill();
        
        int xpToAdd = Mathf.CeilToInt(amount * xpMultiplier);  // Multiply XP by the current multiplier
        currentXP += xpToAdd;
        //Debug.Log($"XP Added: {xpToAdd} (Multiplier: {xpMultiplier}). Total XP: {currentXP}");

        // Check if the player has enough XP to level up
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        UpdateXPBar(xpToAdd, isKill);
    }

    public void UpdateXPBar(int xpAmount, bool isKill)
    {
        float normalizedXP = (float)currentXP / xpToNextLevel;
        PlayerUI.Instance.UpdateXP(currentLevel, normalizedXP, xpAmount,isKill);
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;  // Subtract XP needed for this level
        currentLevel++;  // Increase the player's level

        // Update the XP required for the next level
        xpToNextLevel = GetXPForNextLevel();
        PlayerReferenceManager.Instance.playerController.LevelUp();
        PlayerReferenceManager.Instance.playerStatSystem.UpdateAvailablePoints();
        
        PlayerUI.Instance.LevelUp();
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int GetCurrentXP()
    {
        return currentXP;
    }

    public int GetXPToNextLevel()
    {
        return xpToNextLevel;
    }

    public void OnKill()
    {
        // Increment the XP multiplier on a kill
        xpMultiplier = Mathf.Clamp(xpMultiplier + multiplierIncrement, 1f, maxMultiplier);
        PlayerUI.Instance.SetMultiCooldown(xpMultiplier, multiplierResetCooldown);

        // Increase the kill count during the multiplier
        killedDuringMulti++;
        if (killedDuringMulti > maxKilledDuringMulti)
        {
            maxKilledDuringMulti = killedDuringMulti;
        }


    }

    public void ResetMultiplierAfterDelay()
    {
        xpMultiplier = 1f;  // Reset the multiplier to 1x
        killedDuringMulti = 0;  
    }

    public void SetCurrentLevel(int level)
    {
        currentLevel = level;
    }
    
    public void SetCurrentXP(int xp)
    {
        currentXP = xp;
    }
    
    public void SetXPToNextLevel(int xpToNextLevelIn)
    {
        xpToNextLevel = xpToNextLevelIn;
    }
}
