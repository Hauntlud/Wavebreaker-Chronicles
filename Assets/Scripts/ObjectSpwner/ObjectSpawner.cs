using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObjectChronicleList
{
    public int chronicleIndex;  // The chronicle this list belongs to
    public List<ObjectSpawnerClass> spawnObjects;  // List of objects to spawn in this chronicle
}

[System.Serializable]
public class ObjectSpawnerClass
{
    public GameObject objectPrefab;  // The prefab of the object to spawn
    public int objectsToSpawn;       // The number of instances to spawn
}

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private List<ObjectChronicleList> chronicles;  // List of chronicle object spawns
    [SerializeField] private Vector2 mapSize = new Vector2(50, 50);  // Size of the map (X, Z axes)
    [SerializeField] private float spawnPadding = 2f;  // Minimum distance between objects
    [SerializeField] private LayerMask objectLayer;  // Layer to check for existing objects

    private List<GameObject> activeObjects = new List<GameObject>();  // Track active objects
    private List<GameObject> inactiveObjects = new List<GameObject>();  // Track inactive (triggered) objects



    // Start spawning objects for the specified chronicle
    public void StartChronicleSpawning()
    {
        // Turn off all current active objects
        ResetActiveObjects();

        // Find the list for the current chronicle
        var currentChronicle = chronicles.Find(c => c.chronicleIndex == GameDataManager.Instance.CurrentChronicleIndex);
        if (currentChronicle != null)
        {
            // Spawn or reuse objects for the current chronicle
            SpawnObjects(currentChronicle.spawnObjects);
        }
        else
        {
            Debug.LogWarning("No chronicle found for the current index.");
        }
    }

    // Turn off all active objects and add them to the inactive list
    private void ResetActiveObjects()
    {
        foreach (var obj in activeObjects)
        {
            obj.SetActive(false);  // Deactivate the object
            inactiveObjects.Add(obj);  // Add it back to the inactive list for reuse
        }
        activeObjects.Clear();  // Clear the active objects list
    }

    // Spawn objects for the current chronicle
    private void SpawnObjects(List<ObjectSpawnerClass> objectsToSpawn)
    {
        foreach (var spawn in objectsToSpawn)
        {
            for (int i = 0; i < spawn.objectsToSpawn; i++)
            {
                GameObject newObject;

                // If we have inactive objects, reuse them
                if (inactiveObjects.Count > 0)
                {
                    newObject = inactiveObjects[0];
                    inactiveObjects.RemoveAt(0);  // Remove from inactive list
                    newObject.SetActive(true);  // Reactivate the object
                }
                else
                {
                    // No inactive object available, instantiate a new one
                    Vector3 spawnPosition = GetRandomPositionOnMap();
                    newObject = Instantiate(spawn.objectPrefab, spawnPosition, Quaternion.identity);
                }

                // Set a new position for the object
                newObject.transform.position = GetRandomPositionOnMap();
                activeObjects.Add(newObject);  // Add to active objects list

                // Unsubscribe to prevent double subscription
                var triggerObject = newObject.GetComponent<TriggerObject>();
                triggerObject.OnInteracted -= HandleObjectInteracted;
            
                // Subscribe to interaction event
                triggerObject.OnInteracted += HandleObjectInteracted;
            }
        }
    }

    // Handles when an object is interacted with (triggered)
    private void HandleObjectInteracted(GameObject obj)
    {
        // Deactivate the object and remove it from the active list
        obj.SetActive(false);
        activeObjects.Remove(obj);
        inactiveObjects.Add(obj);  // Add to inactive list for reuse later
    }

    // Get a semi-random position on the map
    private Vector3 GetRandomPositionOnMap()
    {
        Vector3 randomPosition;
        int maxAttempts = 10;
        int attempts = 0;

        do
        {
            float randomX = Random.Range(-mapSize.x / 2, mapSize.x / 2);
            float randomZ = Random.Range(-mapSize.y / 2, mapSize.y / 2);
            randomPosition = new Vector3(randomX, 1, randomZ);

            attempts++;
        }
        while (IsPositionOccupied(randomPosition) && attempts < maxAttempts);

        return randomPosition;
    }

    // Check if the position is already occupied by another object
    private bool IsPositionOccupied(Vector3 position)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, spawnPadding, objectLayer);
        return hitColliders.Length > 0;
    }
}
