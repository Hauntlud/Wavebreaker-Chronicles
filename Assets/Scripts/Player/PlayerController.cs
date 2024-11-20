using System;
using System.Collections;
using UnityEngine;


public class PlayerController : SharedBehaviourCharacters
{
    [SerializeField] private PlayerStatSystem playerStatSystem;  // Reference to the PlayerStatSystem
    
    [SerializeField] private ParticleSystem levelUpPS;
    [SerializeField] private EffectsLifecycleActivation effectsLifecycleActivation;

    private PlayerMovementController playerMovementController;
    
    private bool isMoveSpeedBoosted = false; // Flag to track if a boost is active
    private Coroutine moveSpeedBoostCoroutine;
    
    private void Start()
    {
        SetInheritedTeam(Team.Player);
        characterAudioManager = GetComponent<CharacterAudioManager>();
    }

    public void ApplyMoveSpeedBoost(float duration, float total)
    {
        if (isMoveSpeedBoosted)
        {
            StopCoroutine(moveSpeedBoostCoroutine);
        }
        
        moveSpeedBoostCoroutine = StartCoroutine(MoveSpeedBoostCoroutine(duration,total));
    }

    private IEnumerator MoveSpeedBoostCoroutine(float duration,float total)
    {
        isMoveSpeedBoosted = true;

        // Boost the move speed by 25%
        float originalMoveSpeed = CurrentStats.moveSpeed;
        float boostedMoveSpeed = originalMoveSpeed * total;
        
        playerMovementController.SetMoveSpeed(boostedMoveSpeed);

        // Wait for the duration
        yield return new WaitForSeconds(duration);

        // Revert back to the original move speed
        playerMovementController.SetMoveSpeed(originalMoveSpeed);

        isMoveSpeedBoosted = false;
    }
    
    private void Awake()
    {
        PlayerReferenceManager.Instance.SetPlayerReference(this.transform);
        playerMovementController = GetComponent<PlayerMovementController>();  // Get reference to movement script

    }

    public void AddPlayerHealth(int health)
    {
        PlayerUI.Instance.AddHealth(health);
        AddHealth(health);
        UpdateHealthUI();
    }
    
    public void Initialize()
    {
        PlayerReferenceManager.Instance.playerStatSystem.LoadNewLivePlayerData();
        SetCharacterStats(playerStatSystem.CurrentStats);
        PlayerReferenceManager.Instance.playerAbilityManager.LoadAbilityData();
    }

    public void Spawn()
    {

        PlayerReferenceManager.Instance.SetPlayerReference(this.transform);
        playerStatSystem.SetCurrentSkillPoints();
        playerMovementController.SetMoveSpeed(CurrentStats.moveSpeed);  // Set the move speed to movement controller
        PlayerReferenceManager.Instance.playerCombatController.SetRange(CurrentStats.attackRange);
        effectsLifecycleActivation.StartActivate();
        
        characterAudioManager.PlaySpawnAudio();
        ResetCurrentHealth();
        UpdateHealthUI();
        XPManager.Instance.AddXP(0,false);

        PlayerUI.Instance.LevelUp();
        SetIsDead(false);

    }
    
    public void UpdateStats(CharacterStats.StatValues stats)
    {
        SetCharacterStats(stats);
        PlayerReferenceManager.Instance.PlayerMovementController.SetMoveSpeed(CurrentStats.moveSpeed);
        PlayerReferenceManager.Instance.playerCombatController.SetRange(CurrentStats.attackRange);
    }

    public override void TakeDamage(int amount, GameObject attacker, bool isRetaliatory)
    {
        base.TakeDamage(amount,attacker,isRetaliatory);
        UpdateHealthUI();
        

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    
    
    private void Die()
    {
        if (Isdead) return;

        SetIsDead(true);
        effectsLifecycleActivation.EndDeactivate(false);

        PlayerSpawner.Instance.Death(transform.position);
        DifficultyManager.Instance.ProcessKillResponse(false);
        GameDataManager.Instance.AddDeath();
        characterAudioManager.PlayDeathAudio();
    }

    private void UpdateHealthUI()
    {
        PlayerUI.Instance.UpdateHealth(Mathf.InverseLerp(0, CurrentStats.maxHealth, CurrentHealth));
    }

    public void LevelUp()
    {
        levelUpPS.Play();
        characterAudioManager.PlayLevelUpAudio();
    }
    
}
