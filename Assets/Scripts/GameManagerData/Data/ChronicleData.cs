using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class ChronicleData
{
    [SerializeField, ReadOnly] private int chronicleNumberIndex;
    [SerializeField, ReadOnly] private int mostKills;
    [SerializeField, ReadOnly] private int mostDeath;
    [SerializeField, ReadOnly] private float killToDeathRatio;
    [SerializeField, ReadOnly] private int bestKillStreak;
    [SerializeField, ReadOnly] private float totalPlayTime;
    [SerializeField, ReadOnly] private float rank;
    [SerializeField, ReadOnly] private float highScore;
    [SerializeField, ReadOnly] private float difficulty;

    public int ChronicleNumberIndex => chronicleNumberIndex;
    public int MostKills => mostKills;
    public int MostDeath => mostDeath;
    public float KillToDeathRatio => killToDeathRatio;
    public int BestKillStreak => bestKillStreak;
    public float TotalPlayTime => totalPlayTime;
    public float Rank => rank;
    public float HighScore => highScore;
    public float Difficulty => difficulty;

    public void SetChronicleNumberIndex(int number)
    {
        chronicleNumberIndex = number;
    }
    
    public void SetMostKills(int number)
    {
        mostKills = number;
    }
    
    public void SetMinDeaths(int number)
    {
        mostDeath = number;
    }
    
    public void SetKillToDeathRatio(float number)
    {
        killToDeathRatio = number;
    }
    
    public void SetBestKillStreak(int number)
    {
        bestKillStreak = number;
    }
    
    public void SetTotalPlayTime(float number)
    {
        totalPlayTime = number;
    }
    
    public void SetRank(float rankString)
    {
        rank = rankString;
    }
    
    public void SetHighScore(int number)
    {
        highScore = number;
    }
    
    public void SetDifficulty(float number)
    {
        difficulty = number;
    }

    public ChronicleData DeepCopy()
    {
        return (ChronicleData)this.MemberwiseClone();
    }
}