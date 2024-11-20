using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    public float searchRadius = 10f;
    public LayerMask enemyLayer;

    // This method finds the closest enemy to a given position
    public Transform GetClosestEnemy(Vector3 searchPosition)
    {
        
        Collider[] targetsInRange = Physics.OverlapSphere(searchPosition, searchRadius, enemyLayer);
        Collider closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Collider target in targetsInRange)
        {
            if (!target.CompareTag("Player"))
            {
                float distanceSqr = (searchPosition - target.transform.position).sqrMagnitude;

                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestTarget = target;
                }
            }
        }

        // Return the closest target's Transform if found
        return closestTarget ? closestTarget.transform : null;
    }
}
