using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    
    public static GameDataManager Instance { get; private set; }
    
    [SerializeField] private ChronicleData currentGameData;
    [SerializeField] private List<ChronicleData> currentChronicleGameData;  // Chronicles in the current session
    [SerializeField] private List<ChronicleData> completedChronicleHistory; // Completed chronicles

    [SerializeField] private int currentChronicleIndex = 0;
    [SerializeField] private float totalPlayTime;

    public int CurrentChronicle => currentChronicleIndex + 1;
    public int CurrentChronicleIndex => currentChronicleIndex;
    public ChronicleData CurrentGameData => currentGameData;
    public List<ChronicleData> CurrentChronicleGameData => currentChronicleGameData;
    public List<ChronicleData> CompletedChronicleHistory => completedChronicleHistory;
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
        
        if (currentChronicleGameData == null)
        {
            currentChronicleGameData = new List<ChronicleData>();
        }

        if (completedChronicleHistory == null)
        {
            completedChronicleHistory = new List<ChronicleData>();
        }

        LoadCompletedChronicleHistory();
        LoadGameData();
    }
    
    private void Update()
    {
        if (!PlayerReferenceManager.Instance.PlayerInMenus)
        {
            currentGameData.SetTotalPlayTime(currentGameData.TotalPlayTime + Time.deltaTime);
        }
        
    }

    [Button("Delete History")]
    public void DeleteHistory()
    {
        ClearChronicles();
        completedChronicleHistory.Clear();
        SaveCompletedChronicleHistory();
    }
    
    
    public void ClearChronicles()
    {
        currentChronicleIndex = 0;
        currentChronicleGameData.Clear();
        SaveGameData();
    }

    public ChronicleData GetBestFromSavedChronicles()
    {
        return GetBestScores(CompletedChronicleHistory);
    }
    
    public ChronicleData GetBestScores(List<ChronicleData> chronicleDatas)
    {
    
        ChronicleData bestChronicleData = new ChronicleData();

        foreach (var entry in chronicleDatas)
        {
            // Compare each property and update the best values accordingly
            bestChronicleData.SetMostKills(Math.Max(bestChronicleData.MostKills, entry.MostKills));
            bestChronicleData.SetMinDeaths(Math.Min(bestChronicleData.MostDeath, entry.MostDeath));
            bestChronicleData.SetKillToDeathRatio(Math.Max(bestChronicleData.KillToDeathRatio, entry.KillToDeathRatio)); 
            bestChronicleData.SetBestKillStreak(Math.Max(bestChronicleData.BestKillStreak, entry.BestKillStreak));
            bestChronicleData.SetTotalPlayTime(bestChronicleData.TotalPlayTime + entry.TotalPlayTime);
            bestChronicleData.SetRank(Math.Max(bestChronicleData.Rank, entry.Rank)); 
            bestChronicleData.SetHighScore((int)Math.Max(bestChronicleData.HighScore, entry.HighScore));
        }

        return bestChronicleData;
    }
    
    public ChronicleData GetSummarisedScores()
    {
        ChronicleData summarisedChronicles = new ChronicleData();

        // Variables to accumulate the sums
        int totalKills = 0;
        float totalPlayTime = 0;
        float totalHighScore = 0;

        // Lists to collect values for averaging
        List<float> deathsList = new List<float>();
        List<float> killToDeathRatioList = new List<float>();
        List<float> killStreakList = new List<float>();
        List<float> rankList = new List<float>();
        List<float> difficultyList = new List<float>();

        // Iterate over all chronicles and sum up the data
        foreach (var entry in CurrentChronicleGameData)
        {
            totalKills += entry.MostKills;
            totalPlayTime += entry.TotalPlayTime;
            totalHighScore += entry.HighScore;

            // Collect data for averages
            deathsList.Add(entry.MostDeath);
            killToDeathRatioList.Add(entry.KillToDeathRatio);
            killStreakList.Add(entry.BestKillStreak);
            rankList.Add(entry.Rank);
            difficultyList.Add(entry.Difficulty);
        }

        // Set the cumulative values
        summarisedChronicles.SetMostKills(totalKills);
        summarisedChronicles.SetTotalPlayTime(totalPlayTime);
        summarisedChronicles.SetHighScore(Mathf.RoundToInt(totalHighScore));

        // Calculate averages for other stats
        if (CurrentChronicleGameData.Count > 0)
        {
            summarisedChronicles.SetMinDeaths((int)CalculateAverage(deathsList));  // Average deaths
            summarisedChronicles.SetKillToDeathRatio(CalculateAverage(killToDeathRatioList));  // Average K/D ratio
            summarisedChronicles.SetBestKillStreak((int)CalculateAverage(killStreakList));  // Average kill streak
            summarisedChronicles.SetRank(CalculateAverage(rankList));  // Average rank
            summarisedChronicles.SetDifficulty(CalculateAverage(difficultyList));  // Average difficulty
        }

        return summarisedChronicles;
    }


    public void StartNewChronicle()
    {
        currentGameData = new ChronicleData();
        currentChronicleIndex++;  // Move to the next chronicle

    }

    public void AddKill()
    {
        currentGameData.SetMostKills(currentGameData.MostKills + 1);
    }

    public void AddDeath()
    {
        currentGameData.SetMinDeaths(currentGameData.MostDeath + 1);
    }

    public void SetScore()
    {
        currentGameData.SetHighScore(ScoreManager.Instance.CurrentScore);
    }
    
    public void SetRank()
    {
        currentGameData.SetRank(DifficultyManager.Instance.GetCurrentNormDifficulty());
    }

    public void SetBestKillStreak()
    {
        currentGameData.SetBestKillStreak(XPManager.Instance.MaxKilledDuringMulti);
    }
    
    public void SetDifficulty()
    {
        currentGameData.SetDifficulty(DifficultyManager.Instance.GetCurrentDifficulty());
    }
    
    public void SetKillsToDeath()
    {
        if (currentGameData.MostKills == 0)
        {
            // No kills, Kill-to-Death ratio is 0
            currentGameData.SetKillToDeathRatio(0);
        }
        else if (currentGameData.MostDeath == 0)
        {
            // No deaths, ratio should be set to the total number of kills (as you haven't died)
            currentGameData.SetKillToDeathRatio(currentGameData.MostKills);
        }
        else
        {
            // Normal case: calculate the kill-to-death ratio
            currentGameData.SetKillToDeathRatio((float)currentGameData.MostKills / (float)currentGameData.MostDeath);
        }
    }
    
    public void SetNumber()
    {
        currentGameData.SetChronicleNumberIndex(currentChronicleIndex);
    }


    public void AddChronicleToCurrentChronicleData()
    {
        var checkIfRepeat = false;
        for (int i = 0; i < currentChronicleGameData.Count; i++)
        {
            if (currentChronicleGameData[i].ChronicleNumberIndex == currentGameData.ChronicleNumberIndex)
            {
                checkIfRepeat = true;
            }
        }
        if (checkIfRepeat)currentChronicleGameData.Clear();
        currentChronicleGameData.Add(currentGameData);

        FirebaseEventManager.Instance.LogChronicleEvent(currentGameData.ChronicleNumberIndex + 1, currentGameData.TotalPlayTime);
    }
    
    public void AddChronicleToCompletedChronicles()
    {
        completedChronicleHistory.Add(GetSummarisedScores());
        SaveCompletedChronicleHistory();
    }
    
    public float CalculateAverage(List<float> numbers)
    {
        if (numbers == null || numbers.Count == 0)
        {
            Debug.LogWarning("List is empty or null.");
            return 0f;
        }

        float sum = 0f;
        foreach (var entry in numbers)
        {
            sum += entry;
        }

        return sum / numbers.Count;
    }

    public void SaveGameData()
    {
        GameSaveManager.Instance.SaveCurrentChronicles(currentChronicleGameData, currentChronicleIndex);
    }

    public void LoadGameData()
    {
        GameSaveManager.Instance.LoadCurrentChronicles(out currentChronicleGameData, out currentChronicleIndex);

    }

    public void SaveCompletedChronicleHistory()
    {
        GameSaveManager.Instance.SaveCompletedChronicles(completedChronicleHistory);
    }

    public void LoadCompletedChronicleHistory()
    {
        completedChronicleHistory = GameSaveManager.Instance.LoadCompletedChronicleHistory();
    }

    public ChronicleData GetCurrentChronicleData()
    {
        return currentGameData;
    }
    
    
}
