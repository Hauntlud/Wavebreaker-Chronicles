using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillRowUI : MonoBehaviour
{
    [SerializeField] private Button addButton;  // Button to add pips
    [SerializeField] private Button subtractButton;  // Button to subtract pips
    [SerializeField] private PlayerData.SkillTypes skillType;  // Enum for skill type
    [SerializeField] private TextMeshProUGUI textToUpdate;  // Enum for skill type
    [SerializeField] private List<UIExtension> pips;  // Enum for skill type
    [SerializeField] private Color onColor;  // Enum for skill type
    [SerializeField] private Color offColor;  // Enum for skill type
    
    private PlayerStatSystem playerStatSystem;  // Reference to PlayerStatSystem

    public delegate void PipChangeDelegate();
    public event PipChangeDelegate OnPipChanged;


    private void Start()
    {
        addButton.onClick.AddListener(() => ModifyPip(true));  // Add pip
        subtractButton.onClick.AddListener(() => ModifyPip(false));  // Subtract pip
    }

    public void Initialise()
    {
        playerStatSystem = PlayerReferenceManager.Instance.playerStatSystem;

        UpdateUI();
        SetText();
    }
    
    private void SetText()
    {
        textToUpdate.text = skillType switch
        {
            PlayerData.SkillTypes.Health => "Health",
            PlayerData.SkillTypes.AttackDamage => "Attack Damage",
            PlayerData.SkillTypes.MoveSpeed => "Move Speed",
            PlayerData.SkillTypes.MeleeSpeed => "Melee ATK Speed",
            PlayerData.SkillTypes.RangedSpeed => "Range ATK Speed",
            PlayerData.SkillTypes.AttackRange => "Attack Range",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    private void ModifyPip(bool isAdd)
    {
        if (isAdd)
        {
            // Check if the player has enough points to buy the skill
            if (playerStatSystem.StatBuyingPlayerData.CanAffordToBuy())
            {
                
                playerStatSystem.SpendPoints(skillType);
                OnPipChanged?.Invoke();
            }
        }
        else
        {
            // Allow subtracting only if we have pips greater than the locked-in value
            if (playerStatSystem.LiveGamePlayerData.GetPipOfSkill(skillType) < playerStatSystem.StatBuyingPlayerData.GetPipOfSkill(skillType))
            {
                playerStatSystem.SubtractPoints(skillType);
                OnPipChanged?.Invoke();
            }
        }

        UpdateUI();
        SkillUIManager.Instance.UpdateRowsOfChildren();
    }

    private void TurnOnPips()
    {
        for (int i = 0; i < pips.Count; i++)
        {
            if (playerStatSystem.StatBuyingPlayerData.GetPipOfSkill(skillType) >= i)
            {
                pips[i].Image.color = onColor;
            }
            else
            {
                pips[i].Image.color = offColor;
            }
        }
    }

    public void UpdateUI()
    {
        TurnOnPips();
        addButton.interactable = playerStatSystem.StatBuyingPlayerData.CanAffordToBuy();
        subtractButton.interactable = (playerStatSystem.LiveGamePlayerData.GetPipOfSkill(skillType) < playerStatSystem.StatBuyingPlayerData.GetPipOfSkill(skillType));
    }

    public PlayerData.SkillTypes SkillType => skillType;
}
