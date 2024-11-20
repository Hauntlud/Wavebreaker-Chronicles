using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner Instance { get; private set; }
    
    [SerializeField] private GameObject player;
    [SerializeField] private float respawnTime = 3;
    [SerializeField] private float spawnRadius = 25f;  // Fixed radius around the death location
    [SerializeField] private int maxAttempts = 20;  // Maximum attempts to find a valid spawn position
    [SerializeField] private LayerMask floorLayer; // Assign this in the Inspector to the Floor layer
    
    private Vector3 deathLocation;
    private PlayerController playerController;
    
    private void Awake()
    {
        transform.parent = null;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void DestroyPlayer()
    {
        if (PlayerReferenceManager.Instance.playerController != null)
        {
            Destroy(PlayerReferenceManager.Instance.playerController.gameObject);
        }
    }

    public void InitialisePlayer()
    {
        playerController.Initialize();
    }

    public void SpawnPlayer(Vector3 location)
    {
        if (playerController == null)
        {
            playerController = Instantiate(player).GetComponent<PlayerController>();
            playerController.Initialize();
        }
        else
        {
            Vector3 spawnPosition = GetValidSpawnPosition(location);
            playerController.transform.position = spawnPosition;
            PlayerReferenceManager.Instance.PlayerMovementController.SetPosition(spawnPosition);

        }
        

        playerController.Spawn();
    }
    

    private Vector3 GetValidSpawnPosition(Vector3 center)
    {
        int maxAttempts = 50; // Increased attempts for more reliability

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // Generate a random angle and random distance within the radius
            float angle = Random.Range(0, Mathf.PI * 2);  // Random angle in radians
            float distance = Random.Range(spawnRadius - 5f, spawnRadius + 5f);  // Slight variation around the radius

            // Calculate the offset from the center using the angle and distance
            float xOffset = Mathf.Cos(angle) * distance;
            float zOffset = Mathf.Sin(angle) * distance;

            // Start point slightly above the intended spawn location to raycast downward
            Vector3 spawnPosition = new Vector3(center.x + xOffset, center.y + 10, center.z + zOffset);

            // Raycast downwards to hit the floor layer
            if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, floorLayer))
            {
                if (hit.collider != null && hit.collider.CompareTag("Floor"))
                {
                    // Visualize successful hit point
                    Debug.DrawLine(spawnPosition, hit.point, Color.green, 2.0f);
                    Debug.Log($"Spawn position found at {hit.point} after {attempt + 1} attempts.");
                    return hit.point; // Return the exact point on the floor hit by the ray
                }
            }
            else
            {
                // Visualize failed attempt
                Debug.DrawLine(spawnPosition, spawnPosition + Vector3.down * 10, Color.red, 2.0f);
            }
        }

        // Fallback to center if no valid position is found after max attempts
        Debug.LogWarning("Failed to find a valid spawn position within the radius. Defaulting to center.");
        return center;
    }




    private bool IsPositionOnFloor(Vector3 position)
    {
        RaycastHit hit;
        // Raycast down from a point slightly above the intended spawn position, using only the Floor layer
        if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, floorLayer))
        {
            return hit.collider != null; // True if we hit something in the Floor layer
        }

        return false;
    }


    public void Death(Vector3 deathLocationIn)
    {
        PlayerUI.Instance.DeathCountdown(respawnTime);
        deathLocation = deathLocationIn;
        StartCoroutine(DelaySpawn());
    }

    private IEnumerator DelaySpawn()
    {
        yield return new WaitForSeconds(respawnTime);
        SpawnPlayer(deathLocation);
    }
}
