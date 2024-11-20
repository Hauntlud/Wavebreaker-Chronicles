using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWave", menuName = "Wave/WaveScriptableObject")]
public class WaveScriptableObject : ScriptableObject
{
    public string waveName;  // Name for the wave
    public List<EnemySpawnData> enemySpawns;  // List of enemy types and spawn data
    public float waveStartDelay = 5f;  // Delay before the wave starts
    public float minSpawnDistance;
    public float maxSpawnDistance;
    
    [System.Serializable]
    public class EnemySpawnData
    {
        public string enemyTag;
        public int enemyCount;
        public float spawnRate;
    }
}