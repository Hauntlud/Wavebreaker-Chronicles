using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectsLifecycleActivation : MonoBehaviour
{
    [Header("Start Activate Components")]
    [SerializeField] private List<ParticleSystem> startActivateParticleSystems = new List<ParticleSystem>();
    [SerializeField] private List<MeshRenderer> startVisuals = new List<MeshRenderer>();

    [Header("End Deactivate Components")]
    [SerializeField] private List<ParticleSystem> endDeactivateParticleSystems = new List<ParticleSystem>();
    [SerializeField] private List<MeshRenderer> endVisuals = new List<MeshRenderer>();
    
    [Header("World Components")]
    [SerializeField] private Collider objectCollider;
    [SerializeField] private List<TrailRenderer> trailRenderers = new List<TrailRenderer>();
    

    // Method to handle start activation
    public void StartActivate()
    {
        Reset();  // Reset before activation
        gameObject.SetActive(true);

        foreach (var trail in trailRenderers)
        {
            trail.Clear(); 
            trail.emitting = true;
        }
        
        foreach (var ps in startActivateParticleSystems)
        {
            ps.Play(); 
        }

        foreach (var start in startVisuals)
        {
            start.enabled = true;
        }

        // Enable the collider if present
        if (objectCollider)
        {
            objectCollider.enabled = true;
        }
        
        
    }

    // Method to handle end deactivation
    public void EndDeactivate(bool turnOffObject = true)
    {
        StartCoroutine(HandleEndDeactivate(turnOffObject));
    }

    // Coroutine to handle deactivation effects and wait for them to finish
    private IEnumerator HandleEndDeactivate(bool turnOffObject = true)
    {
        // Disable the collider if present
        if (objectCollider)
        {
            objectCollider.enabled = false;
        }
        
        // Deactivate particle systems simultaneously
        foreach (var ps in endDeactivateParticleSystems)
        {
            ps.Play(); 
        }
        
        foreach (var start in endVisuals)
        {
            start.enabled = false;
        }

        // Wait for all trail renderers to finish
        foreach (var trail in trailRenderers)
        {
            while (trail.time > 0 && trail.HasTrail())
            {
                yield return null;
            }
        }
        
        // Deactivate trail renderers simultaneously
        foreach (var trail in trailRenderers)
        {
            trail.emitting = false;  // Stop emission
        }

        // Wait for all particle systems to finish
        foreach (var ps in endDeactivateParticleSystems)
        {
            while (ps.isPlaying)
            {
                yield return null;
            }
        }
        
        if (turnOffObject)
        {
            // After all effects are done, disable the GameObject
            gameObject.SetActive(false);
        }

    }

    // Private method to handle resetting the object before activation
    private void Reset()
    {
        // Reset trail renderers for future use
        foreach (var trail in trailRenderers)
        {
            trail.Clear();  // Ensure trails are cleared
        }

        // Reset particle systems for future use
        foreach (var ps in startActivateParticleSystems)
        {
            ps.Stop();  // Reset the particle system
        }
    }
}

// Extension method to check if a trail renderer still has active trails
public static class TrailRendererExtensions
{
    public static bool HasTrail(this TrailRenderer trail)
    {
        return trail.positionCount > 0;  // Check if the trail still has active segments
    }
}
