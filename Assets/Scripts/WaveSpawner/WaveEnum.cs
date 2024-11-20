[System.Serializable]
public class Wave
{
    public string name;  // Wave name (for debugging or display)
    public string enemyTag;  // The tag used to identify the enemy in the pooler
    public int enemyCount;  // Number of enemies to spawn
    public float spawnRate;  // How quickly enemies spawn during this wave
}