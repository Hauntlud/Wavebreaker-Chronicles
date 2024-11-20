using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ChronicleList
    {
        public int currentChronicle;
        public List<WaveScriptableObject> waves;  // List of waves in this chronicle
    }

    [SerializeField] private List<ChronicleList> chronicles;  // List of chronicles with their respective waves
    [SerializeField] private WaveSpawnerUI waveSpawnerUI;  // Reference for updating the UI
    
    [SerializeField] private int currentWaveIndex = 0;  // Track the current wave within the chronicle
    private int enemiesRemaining;  // Number of active enemies remaining

    private enum SpawnState { Spawning, Waiting, Idle };
    [SerializeField] private SpawnState state = SpawnState.Idle;

    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private float difficulty;
    
    [SerializeField] private float chronicleDuration = 180f;  // 3 minutes in seconds
    [SerializeField] private float chronicleTimer;

    private List<GameObject> activeEnemies = new List<GameObject>();  // Track all spawned enemies

    private void Start()
    {
        chronicleTimer = chronicleDuration;
    }

    private void Update()
    {
        if (!PlayerReferenceManager.Instance.PlayerInMenus)
        {
            chronicleTimer -= Time.deltaTime;
            waveSpawnerUI.UpdateTimer(chronicleTimer);
            // Check if chronicle timer has expired
            if (chronicleTimer <= 0)
            {
                StartCoroutine(EndCurrentChronicle("Time Ran Out..."));
            }
        }
        
        
    }

    public void ResetWaves()
    {
        currentWaveIndex = 0;
        chronicleTimer = chronicleDuration;
        enemiesRemaining = 0;
        state = SpawnState.Idle;
        waveSpawnerUI.UpdateTimer(chronicleTimer);
        waveSpawnerUI.UpdateUI(currentWaveIndex, 4);
        waveSpawnerUI.UpdateWaveSkill();
    }

    private Vector3 GetValidNavMeshPosition(float minSpawn, float maxSpawn)
    {
        Vector3 playerPos = PlayerReferenceManager.Instance.playerController.transform.position;
        Vector3 fallbackPosition = playerPos + (Random.insideUnitSphere * minSpawn);
        fallbackPosition.y = playerPos.y;

        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector3 randomPosition = GetRandomSpawnPosition(minSpawn, maxSpawn);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.LogWarning("Failed to find valid NavMesh position, using fallback position.");
        return fallbackPosition;
    }

    private IEnumerator SpawnWave(List<WaveScriptableObject> waves)
    {
        if (GameDataManager.Instance.CurrentChronicleIndex != 0)
        {
            SkillUIManager.Instance.IfSpendPointsOpenSkillUI();
        }
        
        state = SpawnState.Spawning;
        enemiesRemaining = 0;
        difficulty = DifficultyManager.Instance.GetCurrentDifficulty();
        waveSpawnerUI.UpdateWaveSkill();
        float spawnInterval = 1.0f;  // Adjust the interval time between each enemy spawn

        List<WaveScriptableObject.EnemySpawnData> spawnData = waves[currentWaveIndex].enemySpawns.ToList();

        foreach (var enemyData in spawnData)
        {
            for (int i = 0; i < enemyData.enemyCount; i++)
            {
                // Pause the coroutine if the player is in the menu
                while (PlayerReferenceManager.Instance.PlayerInMenus)
                {
                    yield return null; // Wait until PlayerInMenus is false
                }

                if (state == SpawnState.Idle) yield break; // Exit if state changes to Idle

                SpawnEnemy(enemyData.enemyTag, waves[currentWaveIndex]);
                waveSpawnerUI.UpdateUI(currentWaveIndex + 1, waves.Count);
                enemiesRemaining++;
                yield return new WaitForSeconds(spawnInterval);  // Wait before the next enemy spawns
            }
        }

        state = SpawnState.Waiting;  // All enemies spawned, waiting for them to be cleared
    }

    private void SpawnEnemy(string enemyTag, WaveScriptableObject wave)
    {
        GameObject enemy = MultiObjectPooler.Instance.SpawnFromPool(enemyTag, GetValidNavMeshPosition(wave.minSpawnDistance, wave.maxSpawnDistance), quaternion.identity);
        if (enemy != null)
        {
            var tempEnemy = enemy.GetComponent<EnemyController>();
            tempEnemy.OnEnemyDeath += HandleEnemyDeath;
            tempEnemy.Spawn(difficulty);

            // Track the spawned enemy
            activeEnemies.Add(enemy);
        }
    }

    private void HandleEnemyDeath(GameObject enemy)
    {
        enemiesRemaining--;

        enemy.GetComponent<EnemyController>().OnEnemyDeath -= HandleEnemyDeath;
        activeEnemies.Remove(enemy);  // Remove from the active enemies list

        if (enemiesRemaining == 0 && state == SpawnState.Waiting)
        {
            currentWaveIndex++;
            if (currentWaveIndex < chronicles[GameDataManager.Instance.CurrentChronicleIndex].waves.Count)
            {
                StartCoroutine(SpawnWave(chronicles[GameDataManager.Instance.CurrentChronicleIndex].waves));
            }
            else
            {
                if (GameDataManager.Instance.CurrentChronicleIndex == 3)
                {
                    StartCoroutine(EndCurrentChronicle("All Chronicle's Completed!"));
                }
                else
                {
                    StartCoroutine(EndCurrentChronicle("Chronicle Completed!"));
                }
            }
        }
    }

    private IEnumerator EndCurrentChronicle(string waveStatus)
    {
        while (PlayerReferenceManager.Instance.PlayerInMenus)
        {
            yield return null; // Wait until PlayerInMenus is false
        }
        
        if (state != SpawnState.Idle)
        {
            Debug.Log($"Ending chronicle: {waveStatus}");
            state = SpawnState.Idle;
            enemiesRemaining = 0;
            
            // Clean up all active enemies before proceeding
            CleanupRemainingEnemies();

            GameManager.Instance.EndChronicle(waveStatus);
        }
    }

    public void ProceedToNextChronicle()
    {
        currentWaveIndex = 0;
        if (GameDataManager.Instance.CurrentChronicleIndex < chronicles.Count)
        {
            Debug.Log($"Proceeding to Chronicle {GameDataManager.Instance.CurrentChronicleIndex}");
            chronicleTimer = chronicleDuration;
            state = SpawnState.Waiting;

            // Clean up any leftover enemies
            CleanupRemainingEnemies();

            StartCoroutine(SpawnWave(chronicles[GameDataManager.Instance.CurrentChronicleIndex].waves));
        }
        else
        {
            Debug.Log("All chronicles completed!");
            state = SpawnState.Idle;
        }
    }

    public void TurnOn()
    {
        state = SpawnState.Waiting;

        // Clean up any leftover enemies
        CleanupRemainingEnemies();

        StartCoroutine(SpawnWave(chronicles[GameDataManager.Instance.CurrentChronicleIndex].waves));
    }

    private Vector3 GetRandomSpawnPosition(float minSpawn, float maxSpawn)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float distance = Random.Range(minSpawn, maxSpawn);
        Vector3 playerPos = PlayerReferenceManager.Instance.playerController.transform.position;

        return new Vector3(
            playerPos.x + Mathf.Cos(angle) * distance,
            playerPos.y,
            playerPos.z + Mathf.Sin(angle) * distance
        );
    }

    // Method to return all active enemies to the pool
    private void CleanupRemainingEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            enemy.SetActive(false);
        }
        activeEnemies.Clear();  // Clear the list after cleanup
    }
}
