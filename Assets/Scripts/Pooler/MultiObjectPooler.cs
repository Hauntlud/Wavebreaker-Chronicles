using System.Collections.Generic;
using UnityEngine;

public class MultiObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;  // Tag to identify the object type
        public GameObject prefab;  // Prefab of the object to pool
        public int size;  // Initial size of the pool
    }

    public static MultiObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Pool> pools;  // List of pools for different objects
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Create pools for each object type
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // Method to spawn an object from the pool based on its tag
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        Queue<GameObject> objectPool = poolDictionary[tag];

        // Check for inactive objects in the pool
        foreach (GameObject obj in objectPool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                //print("Turning on " + obj.name);
                obj.transform.position = position;
                obj.transform.rotation = rotation;

                return obj;
            }
        }

        // If all objects are active, create a new one, add it to the pool and return it
        Pool pool = pools.Find(p => p.tag == tag);
        if (pool != null)
        {
            GameObject newObj = Instantiate(pool.prefab, position, rotation);
            newObj.SetActive(true);
            objectPool.Enqueue(newObj); // Add to the pool for future use
            poolDictionary[tag] = objectPool; // Update the dictionary with the modified pool
            return newObj;
        }

        Debug.LogWarning("No pool found with the tag " + tag);
        return null;
    }
}


