using System;
using UnityEngine;
using System.Collections;
using NaughtyAttributes;
using UnityEngine.Events;

public class MeleeAttackController : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask targetLayers;
    
    [SerializeField] private ParticleSystem frontParticles;
    [SerializeField] private LocalZAxisRotation localZRotation;
    [SerializeField] private ScaleToNorm scaleToNorm;
    [SerializeField] private Vector3 rotationOffset;
    private SharedBehaviourCharacters sharedBehaviourCharacters;

    public UnityEvent triggeredWhenFired;

    private void Start()
    {
        sharedBehaviourCharacters = GetComponent<SharedBehaviourCharacters>();
    }

    public void PerformMeleeAttack(Vector3 boxDimension, Team theirTeam, int attackDamage, CharacterAudioManager characterAudioManager,float attackRangeNorm)
    {
        // Show the attack telegraph and perform the attack
        ExecuteMeleeAttack(boxDimension, theirTeam, attackDamage,characterAudioManager,attackRangeNorm);
    }

    private void ExecuteMeleeAttack(Vector3 boxDimension, Team theirTeam, int attackDamage, CharacterAudioManager characterAudioManager, float attackRangeNorm)
    {

        // Calculate the center position of the box based on the character's forward direction and the box dimensions
        Vector3 boxCenter = attackPoint.position + attackPoint.forward * (boxDimension.z / 2);

        // Create the box area in front of the character to check for targets
        Collider[] hitTargets = Physics.OverlapBox(boxCenter, boxDimension / 2, attackPoint.rotation, targetLayers);

        foreach (Collider target in hitTargets)
        {
            SharedBehaviourCharacters targetable = target.GetComponent<SharedBehaviourCharacters>();

            if (targetable != null && theirTeam != targetable.GetTeam())
            {
                targetable.TakeDamage(attackDamage, gameObject, sharedBehaviourCharacters.AutoRetaliateOn);
                if (theirTeam == Team.Player)
                {
                    PlayerReferenceManager.Instance.playerController.AddPlayerHealth(5);
                }
            }
        }

        triggeredWhenFired.Invoke();
        characterAudioManager.PlayMeleeAttackAudio();
        frontParticles.Play();
        scaleToNorm.ScaleToNormFunction(attackRangeNorm);
    }

    
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;

        // Calculate the center of the box based on the box dimensions and the attack point's forward direction
        Vector3 boxDimension = PlayerReferenceManager.Instance.playerController.CurrentStats.boxDimensions;
        Vector3 boxCenter = attackPoint.position + attackPoint.forward * (boxDimension.z / 2);

        // Store the current transform (position, rotation, scale) of the attack point
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(boxCenter, attackPoint.rotation, transform.localScale);

        // Apply the matrix to the Gizmo
        Gizmos.matrix = rotationMatrix;

        // Draw the wireframe box with the specified size, centered at the calculated box center
        Gizmos.DrawWireCube(Vector3.zero, boxDimension);
    }

    [Button("Find AttackPoint")]
    public void FindAttackPoint()
    {
        var children = GetComponentsInChildren<Transform>();

        foreach (var child in children)
        {
            if (child.transform.CompareTag("AttackPoint"))
            {
                attackPoint = child.transform;
            }
        }
    }
}