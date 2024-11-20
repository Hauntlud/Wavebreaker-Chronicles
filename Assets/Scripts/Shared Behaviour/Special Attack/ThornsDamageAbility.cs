using System.Collections;
using UnityEngine;

public class ThornsDamageAbility : SpecialAbilityBehavior
{
    private string abilityName = "ThornsDamageAbility";
    private PlayerController playerController;
    private EnemyController enemyController;
    private ParticleSystem particleSystem;
    private SharedBehaviourCharacters sharedBehaviourCharacters;
    
    private LineRenderer lineRenderer;
    private Transform attackerTransform;
    private Coroutine disableLineRendererCoroutine;

    private void Start()
    {
        // Get SharedBehaviourCharacters and subscribe to the OnPlayerHit event
        playerController = GetComponent<PlayerController>();
        sharedBehaviourCharacters = GetComponent<SharedBehaviourCharacters>();

        sharedBehaviourCharacters.SetAutoRetaliateOn(true);
        
        // Subscribe to the event and log for verification
        sharedBehaviourCharacters.OnCharacterHit += CharacterHit;
        
        if (specialAbility == null)
        {
            specialAbility = AbilitySingleton.Instance.GetAbilityData(abilityName);
        }

        if (particleSystem == null)
        {
            if (sharedBehaviourCharacters.GetTeam() == Team.Enemy)
            {
                particleSystem = Instantiate(specialAbility.particleEffectEnemy).GetComponent<ParticleSystem>();
            }
            else
            {
                particleSystem = Instantiate(specialAbility.particleEffectPlayer).GetComponent<ParticleSystem>();
            }
        }

        particleSystem.transform.parent = gameObject.transform;
        
        // Set up the LineRenderer
        CreateLineRenderer();
    }
    
    private void Update()
    {
        // Continuously update the line to follow the player and attacker
        if (lineRenderer.enabled && attackerTransform != null)
        {
            lineRenderer.SetPosition(0, transform.position + Vector3.up);      // Player position
            lineRenderer.SetPosition(1, attackerTransform.position + Vector3.up); // Attacker position
        }
    }

    private void CreateLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;  // Two points: start and end
        lineRenderer.startWidth = 0.3f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.numCapVertices = 5;
        lineRenderer.numCapVertices = 5;

        if (sharedBehaviourCharacters.GetTeam() == Team.Player)
        {
            lineRenderer.material = new Material(specialAbility.playerMaterial);
        }
        else
        {
            lineRenderer.material = new Material(specialAbility.enemymaterial);
        }
        

        lineRenderer.enabled = false; // Start disabled
        SetLineColorBasedOnTeam();
    }

    private void SetLineColorBasedOnTeam()
    {
        if (lineRenderer.material != null)
        {
            Color lineColor;
            if (sharedBehaviourCharacters.GetTeam() == Team.Enemy)
            {
                lineColor = new Color(1f, 0f, 0f, 0.7f); // Red with 70% opacity
            }
            else
            {
                lineColor = new Color(0f, 0f, 1f, 0.7f); // Blue with 70% opacity
            }

            // Set line color and emission
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            
        }
    }


    private void CharacterHit(GameObject attacker)
    {
        //Debug.Log("ThornsDamageAbility retaliating against " + attacker.gameObject.name);

        // Store the attacker reference
        attackerTransform = attacker.transform;

        SharedBehaviourCharacters attackerCharacter = attacker.GetComponent<SharedBehaviourCharacters>();
        if (attackerCharacter != null)
        {
            // Apply retaliatory damage
            attackerCharacter.TakeDamage(sharedBehaviourCharacters.CurrentStats.attackDamage / 4, gameObject, isRetaliatory: true);
            sharedBehaviourCharacters.characterAudioManager.PlayRandomClipFromCollection(specialAbility.specialAudioCollection);
            
            // Set line positions and enable the LineRenderer
            lineRenderer.SetPosition(0, transform.position + Vector3.up);
            lineRenderer.SetPosition(1, attackerTransform.position + Vector3.up);
            lineRenderer.enabled = true;

            // Stop any existing coroutine and start a new one to disable the LineRenderer after a delay
            if (disableLineRendererCoroutine != null)
            {
                StopCoroutine(disableLineRendererCoroutine);
            }
            disableLineRendererCoroutine = StartCoroutine(DisableLineRendererAfterDelay(0.5f));
        }
    }
    
    private IEnumerator DisableLineRendererAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Disable line renderer and clear attacker reference
        lineRenderer.enabled = false;
        attackerTransform = null;
    }


    public override void Activate()
    {
    }
    
    public void OnDestroy()
    {
        // Unsubscribe from event to avoid memory leaks
        if (sharedBehaviourCharacters != null)
        {
            sharedBehaviourCharacters.OnCharacterHit -= CharacterHit;
        }
    }
    
}