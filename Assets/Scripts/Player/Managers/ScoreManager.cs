using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; } // Singleton instance
    
    
    [SerializeField] private int baseScore = 15;
    [SerializeField] private int currentScore = 0;
    public int CurrentScore => currentScore;
    private void Awake()
    {
        transform.parent = null;
        // Ensure the singleton is set
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    
    public void AddScore(float difficulty, bool isKill)
    {
        difficulty = Mathf.Clamp(difficulty, 0.1f, 1);
        int xpToAdd = Mathf.CeilToInt((baseScore * difficulty) * XPManager.Instance.XpMultiplier);  // Multiply XP by the current multiplier
        currentScore += xpToAdd;

        PlayerUI.Instance.SetScore(currentScore);
    }
    
}
