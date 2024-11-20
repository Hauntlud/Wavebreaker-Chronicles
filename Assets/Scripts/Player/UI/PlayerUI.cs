using System;
using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { get; private set; }

    [SerializeField] private UIExtension healthBarExtension;
    [SerializeField] private UIExtension deathCountdown;
    [SerializeField] private UIExtension xpbar;
    [SerializeField] private UIExtension levelupShake;
    [SerializeField] private UIExtension levelupTitleText;
    [SerializeField] private UIExtension multiCooldown;
    [SerializeField] private UIExtension spendPoints;
    [SerializeField] private UIExtension currentScore;
    
    [SerializeField] private List<GameObject> pcUI;
    [SerializeField] private List<GameObject> mobileUI;
    [SerializeField] private TextMeshProUGUI movementText;
    [SerializeField] private TextMeshProUGUI rotationText;
    
    [SerializeField] private Sprite meleeSprite;
    [SerializeField] private Sprite rangeSprite;
    [SerializeField] private UIExtension mainSelectedCooldown;
    [SerializeField] private UIExtension secondaryCooldown;
    [SerializeField] private UIExtension mainUIRef;
    [SerializeField] private UIExtension secondaryUIRef;
    [SerializeField] private Image mainSelectedSprite;
    [SerializeField] private Image secondarySprite;
    
    public DamageNumber numberPrefab;
    public DamageNumber healthNumberPrefab;
    public RectTransform rectParent;
    public RectTransform healthRect;
    
    private float totalTime; // Duration for the cooldown
    private float elapsedTime; // Tracks the lerp progress
    private bool isLerping = false;
    
    private Coroutine propertyCoroutine;  // Single coroutine for both properties
    private void Awake()
    {
        transform.parent = null;
        // Check if instance already exists and destroy the duplicate, or assign this instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep this instance across scenes
        }
    }

    private void Start()
    {
        if (PlayerReferenceManager.Instance.IsMobile)
        {
            foreach (var gameObject in mobileUI)
            {
                gameObject.GetComponent<TextMeshProUGUI>().text = "Switch Weapon";
            }

            movementText.text = "Movement";
            rotationText.text = "Closest Target";
        }
        else
        {
            foreach (var gameObject in pcUI)
            {
                gameObject.GetComponent<TextMeshProUGUI>().text = "Spacebar";
            }
            
            movementText.text = "Movement";
            rotationText.text = "Closest Target";
        }
    }
    
    private void Update()
    {
        // Only lerp if we are actively lerping and the player is not in the UI
        if (isLerping && !PlayerReferenceManager.Instance.PlayerInMenus)
        {
            // Update lerp progress based on elapsed time
            elapsedTime += Time.deltaTime;
            float lerpProgress = Mathf.Clamp01(elapsedTime / totalTime);
            multiCooldown.Image.fillAmount = 1f - lerpProgress;

            // Stop lerping once we reach the end
            if (lerpProgress >= 1f)
            {
                isLerping = false;
                XPManager.Instance.ResetMultiplierAfterDelay();
            }
        }
    }

    public void AddHealth(int healthAmount)
    {
        healthNumberPrefab.SpawnGUI(healthRect, Vector2.zero, "+" + healthAmount);
    }
    
    public void SpendPointsUI()
    {
        //Debug.Log("LevelUp called, CurrentSkillPoints: " + PlayerReferenceManager.Instance.playerStatSystem.EnoughToBuy());
        
        if (PlayerReferenceManager.Instance.playerStatSystem.EnoughToBuy())
        {
            spendPoints.FadeIn();
            spendPoints.ShakeUIElement();
            spendPoints.Button.interactable = true;
        }
        else
        {
            spendPoints.FadeOut();
            spendPoints.Button.interactable = false;
        }
        
    }
    
    public void SetScore(float score)
    {
        currentScore.LerpToNumber(score);
    }
    
    public void SetMultiCooldown(float multiplier, float cooldownDuration)
    {
        // Update the multiplier display text
        multiCooldown.TextPro.text = $"X{multiplier:F1}";

        // Reset lerp variables if starting a new cooldown
        totalTime = cooldownDuration;
        elapsedTime = 0f;
        isLerping = true;
        multiCooldown.Image.fillAmount = 1f; // Start at full
    }
    
    public void SwitchAttacks()
    {
        if (PlayerReferenceManager.Instance.playerCombatController.IsMeleeSelected)
        {
            // Melee is now the primary attack
            mainSelectedSprite.sprite = meleeSprite;
            secondarySprite.sprite = rangeSprite;

            // Set mainSelectedCooldown to Melee and secondaryCooldown to Ranged
            mainSelectedCooldown = mainUIRef;
            secondaryCooldown = secondaryUIRef;
            
        }
        else
        {
            // Ranged is now the primary attack
            mainSelectedSprite.sprite = rangeSprite;
            secondarySprite.sprite = meleeSprite;

            // Set mainSelectedCooldown to Ranged and secondaryCooldown to Melee
            mainSelectedCooldown = secondaryUIRef;
            secondaryCooldown = mainUIRef;
            
        }
    }
    
    public void MeleeCooldown(float cooldown)
    {
        // Calculate the remaining fill amount
        float remainingFill = Mathf.Clamp01(cooldown / PlayerReferenceManager.Instance.playerController.CurrentStats.meleeAttackSpeed);
    
        if (cooldown <= 0)
        {
            // Cooldown finished, reset to full
            mainSelectedCooldown.Image.fillAmount = 1;
        }
        else
        {
            // Set the fill amount based on the remaining cooldown
            mainSelectedCooldown.Image.fillAmount = remainingFill;
        }
    }

    public void RangeCooldown(float cooldown)
    {
        // Calculate the remaining fill amount
        float remainingFill = Mathf.Clamp01(cooldown / PlayerReferenceManager.Instance.playerController.CurrentStats.rangedAttackSpeed);
    
        if (cooldown <= 0)
        {
            // Cooldown finished, reset to full
            secondaryCooldown.Image.fillAmount = 1;
        }
        else
        {
            // Set the fill amount based on the remaining cooldown
            secondaryCooldown.Image.fillAmount = remainingFill;
        }
    }



    public void UpdateXP(int level, float normNumber, int xpAmount, bool isText)
    {
        if (isText)
        {
            DamageNumber damageNumber = numberPrefab.SpawnGUI(rectParent, Vector2.zero, "+" + xpAmount);
        }
        levelupShake.TextPro.text = level.ToString();
        xpbar.LerpFillAmount(normNumber);
    }

    public void LevelUp()
    {
        levelupShake.ShakeUIElement();
        levelupTitleText.ScaleUpAndDown();
        SpendPointsUI();
    }
    
    public void UpdateHealth(float normAmount)
    {
        healthBarExtension.LerpFillAmount(normAmount);
    }
    
    public void DeathCountdown(float totalTime)
    {
        deathCountdown.SetLerpDuriation(totalTime);
        deathCountdown.Image.fillAmount = 1;
        deathCountdown.LerpFillAmount(0);
    }
}