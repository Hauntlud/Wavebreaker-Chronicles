using System;
using NaughtyAttributes;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private MeleeAttackController meleeAttackController;
    [SerializeField] private RangedAttackController rangedAttackController;
    [SerializeField] private float timerInterval = 0.3f;  // The interval in seconds for the timer (0.3 seconds)
    
    private float lastMeleeAttackTime = -Mathf.Infinity;
    private float lastRangedAttackTime = -Mathf.Infinity;
    [SerializeField] private bool isMeleeSelected = true;
    public bool IsMeleeSelected => isMeleeSelected;
    
    private float timer = 0f;  // Timer to track time intervals

    
    [Button("Set References Components")]
    private void Reset()
    {
        playerController = GetComponent<PlayerController>();
        meleeAttackController = GetComponent<MeleeAttackController>();
        rangedAttackController = GetComponent<RangedAttackController>();
    }

    private void Start()
    {
        ChangeAttacks();
    }

    private void Update()
    {
        if (playerController.Isdead || PlayerReferenceManager.Instance.PlayerInMenus) return;
        HandleInput();
        HandleTimer();  // Check and update the timer
        
        // Continuously update cooldowns for both melee and ranged
        UpdateCooldowns();
    }
    
    private void HandleTimer()
    {
        // Update the timer
        timer += Time.deltaTime;

        // Check if 0.3 seconds have passed
        if (timer >= timerInterval)
        {
            // Trigger whatever action you want to execute every 0.3 seconds
            if (isMeleeSelected)
            {
                PerformMeleeAttack();
            }
            else
            {
                PerformRangedAttack();
            }

            // Reset the timer
            timer = 0f;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Code to execute when the spacebar is pressed
            ChangeAttacks();
        }

    }

    public void PerformMeleeAttack()
    {
        if (Time.time >= lastMeleeAttackTime + playerController.CurrentStats.meleeAttackSpeed)
        {
            meleeAttackController.PerformMeleeAttack(
                playerController.CurrentStats.boxDimensions,
                playerController.GetTeam(),
                playerController.CurrentStats.attackDamage,
                playerController.characterAudioManager,
                playerController.CurrentStats.attackRange / playerController.GetStats().DifficultyRanges.attackRangeMax
            );  // Trigger the melee attack

            lastMeleeAttackTime = Time.time;  // Reset cooldown timer
            PlayerUI.Instance.MeleeCooldown(playerController.CurrentStats.meleeAttackSpeed);
        }
    }

    public void PerformRangedAttack()
    {
        if (Time.time >= lastRangedAttackTime + playerController.CurrentStats.rangedAttackSpeed)
        {
            rangedAttackController.FireProjectile(playerController.characterAudioManager);  // Trigger the ranged attack
            lastRangedAttackTime = Time.time;  // Reset cooldown timer
            PlayerUI.Instance.RangeCooldown(playerController.CurrentStats.rangedAttackSpeed);
        }
    }
    
    private void UpdateCooldowns()
    {
        // Calculate remaining cooldowns for both melee and ranged attacks
        float remainingMeleeCooldown = Mathf.Max(0, playerController.CurrentStats.meleeAttackSpeed - (Time.time - lastMeleeAttackTime));
        float remainingRangedCooldown = Mathf.Max(0, playerController.CurrentStats.rangedAttackSpeed - (Time.time - lastRangedAttackTime));

        // Update the UI with the remaining cooldowns
        if (isMeleeSelected)
        {
            // Melee is primary
            PlayerUI.Instance.MeleeCooldown(remainingMeleeCooldown);
            PlayerUI.Instance.RangeCooldown(remainingRangedCooldown);
        }
        else
        {
            // Ranged is primary
            PlayerUI.Instance.RangeCooldown(remainingRangedCooldown);
            PlayerUI.Instance.MeleeCooldown(remainingMeleeCooldown);
        }
    }

    public void SetRange(float range)
    {
        rangedAttackController.SetDestroyRange(range);
    }

    public void ChangeAttacks()
    {
        isMeleeSelected = !isMeleeSelected;
        PlayerUI.Instance.SwitchAttacks();

        // Calculate the remaining cooldown for each attack type
        float remainingMeleeCooldown = Mathf.Max(0, playerController.CurrentStats.meleeAttackSpeed - (Time.time - lastMeleeAttackTime));
        float remainingRangedCooldown = Mathf.Max(0, playerController.CurrentStats.rangedAttackSpeed - (Time.time - lastRangedAttackTime));

        // Update both cooldown UIs
        PlayerUI.Instance.MeleeCooldown(remainingMeleeCooldown);
        PlayerUI.Instance.RangeCooldown(remainingRangedCooldown);
    }
    

}