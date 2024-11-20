using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SharedBehaviourCharacters : MonoBehaviour
{
    [FormerlySerializedAs("characterStatsSaved")]
    [SerializeField] private CharacterStats characterStatsData;
    [SerializeField] private CharacterStats.StatValues currentStats;
    [SerializeField] private Image currentHealthImage;
    
    public CharacterAudioManager characterAudioManager;
    public CharacterStats.StatValues CurrentStats => currentStats;
    public CharacterStats CharacterStatsData => characterStatsData;

    [SerializeField] private int currentHealth = 0;
    [SerializeField] private bool autoRetaliateOn;
    public bool AutoRetaliateOn => autoRetaliateOn;
    public int CurrentHealth => currentHealth;
    public event Action<GameObject> OnCharacterHit; 
    
    public bool Isdead => isDead;

    private bool thornsEnabled;
    private bool isDead = false;
    private Team team;
    public Team Team => team;
    public object Debug { get; set; }

    public void AddHealth(int health)
    {
        currentHealth += health;
        characterAudioManager.PlayHealingAudio();
    }
    protected void SetIsDead(bool isDeadIn)
    {
        isDead = isDeadIn;
    }
    protected void SetCharacterStats(CharacterStats.StatValues currentStatsIn)
    {
        currentStats = currentStatsIn.DeepCopy();
    }

    protected void SubtractHealth(int damage)
    {
        currentHealth -= Mathf.Clamp(damage, 0, currentStats.maxHealth);
        characterAudioManager.PlayDamagedAudio();
        UpdateHealthIndicator();
    }
    
    public virtual void TakeDamage(int amount, GameObject attacker, bool isRetaliatory)
    {
        SubtractHealth(amount);
    
        // Only invoke OnCharacterHit if this isn't retaliatory damage
        if (attacker != null && !isRetaliatory)
        {
            OnCharacterHit?.Invoke(attacker);
            //print("AUTO RETALIATE");
        }
    }

    private void UpdateHealthIndicator()
    {
        if (currentHealthImage == null) return;
        currentHealthImage.fillAmount = (float)currentHealth / (float)currentStats.maxHealth;
    }

    protected void ResetCurrentHealth()
    {
        currentHealth = currentStats.maxHealth;
        UpdateHealthIndicator();
    }

    public void SetInheritedTeam(Team teamIn)
    {
        team = teamIn;
    }
    
    public Team GetTeam()
    {
        return Team;
    }

    public CharacterStats.StatValues GetStats()
    {
        return CurrentStats;
    }

    public void SetAutoRetaliateOn(bool toggle)
    {
        autoRetaliateOn = toggle;
    }
    
}
