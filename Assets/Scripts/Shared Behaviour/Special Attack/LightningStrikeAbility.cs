using UnityEngine;

public class LightningStrikeAbility : SpecialAbilityBehavior
{
    private string abilityName = "LightningStrikeAbility";
    [SerializeField] private ParticleSystem lightningVFX; // Assign your VFX prefab in the inspector

    private SharedBehaviourCharacters sharedBehaviourCharacters;
    private Team team;

    private void Start()
    {
        sharedBehaviourCharacters = GetComponent<SharedBehaviourCharacters>();
        team = sharedBehaviourCharacters.Team;

        if (specialAbility == null)
        {
            specialAbility = AbilitySingleton.Instance.GetAbilityData(abilityName);
        }

        if (lightningVFX == null)
        {
            if (team == Team.Player)
            {
                lightningVFX = Instantiate(specialAbility.particleEffectPlayer).GetComponent<ParticleSystem>();
            }
            else
            {
                lightningVFX = Instantiate(specialAbility.particleEffectEnemy).GetComponent<ParticleSystem>();
            }
        }
    }
    
    private void Update()
    {
        if (!PlayerReferenceManager.Instance.PlayerInMenus && !sharedBehaviourCharacters.Isdead)
        {
            UpdateCooldown(Time.deltaTime);
        }
    }

    public override void Activate()
    {
        //Debug.Log($"{specialAbility.abilityName} activated! Performing Lightning Strike.");

        // Perform a sphere cast to find the first target within the radius
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, specialAbility.abilityStats.range, Vector3.up);
        
        foreach (var hit in hits)
        {
            // Check if the hit object has a SharedBehaviourCharacters component and is on a different team
            SharedBehaviourCharacters targetCharacter = hit.collider.GetComponent<SharedBehaviourCharacters>();

            if (targetCharacter != null && targetCharacter.Team != team && !targetCharacter.Isdead)
            {
                // Trigger VFX at the target position
                lightningVFX.gameObject.transform.position = targetCharacter.transform.position + new Vector3(0, 1, 0);
                lightningVFX.Play();

                // Apply damage to the character
                var sharedBehaviour = targetCharacter.GetComponent<SharedBehaviourCharacters>();
                sharedBehaviour.TakeDamage(sharedBehaviourCharacters.CurrentStats.attackDamage,gameObject,sharedBehaviourCharacters.AutoRetaliateOn);
                sharedBehaviour.characterAudioManager.PlayRandomClipFromCollection(specialAbility.specialAudioCollection);
                
                // Only hit the first valid target
                break;
            }
        }
    }
}