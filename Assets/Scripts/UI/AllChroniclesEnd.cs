using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllChroniclesEnd : MonoBehaviour
{
    public static AllChroniclesEnd Instance { get; private set; }
    
    [SerializeField] private UIExtension endChronicles;  
    [SerializeField] private List<AllChronicleEndRow> chronicleRows;  // Rows to display the data

    [SerializeField] private Button playAgainButton; 
    [SerializeField] private Button quitButton; 
    
    private void Awake()
    {
        // Ensure there's only one instance of the PlayerReferenceManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Ensure the reference persists across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        playAgainButton.onClick.AddListener(BackToMainMenu);
        quitButton.onClick.AddListener(QuitButton);
    }

    public void BackToMainMenu()
    {
        PlayerReferenceManager.Instance.SetPlayerInMenus(false);
        endChronicles.FadeOut();
        GameDataManager.Instance.LoadCompletedChronicleHistory();
        GameManagerUI.Instance.ResetChronicles();
        GameManagerUI.Instance.ToggleUIMenu(true);

    }

    public void QuitButton()
    {
        
        GameManagerUI.Instance.QuitGame();
    }

    public void UpdateAllChronicles()
    {
        PlayerReferenceManager.Instance.SetPlayerInMenus(true,"AllChronicleEnd");
        endChronicles.FadeIn();
        // Get the old best scores before adding new chronicles
        var oldBestScores = GameDataManager.Instance.GetSummarisedScores();
        
        // Get the new best scores after adding the new chronicles
        var newBestScores = GameDataManager.Instance.GetBestScores(GameDataManager.Instance.CompletedChronicleHistory);
        
        // Add the new chronicle data to the GameDataManager
        GameDataManager.Instance.AddChronicleToCompletedChronicles();
        
        // Update UI rows based on changes between old and new best scores
        StartCoroutine(UpdateRows(oldBestScores, newBestScores));
    }

    private IEnumerator UpdateRows(ChronicleData oldBest, ChronicleData newBest)
    {
        if (chronicleRows == null || chronicleRows.Count == 0)
        {
            Debug.LogError("No chronicle rows assigned.");
            yield return null;
        }

        // Update each row directly with the old and new best scores.
        // Assuming each row corresponds to a specific stat in the UI (e.g., Kills, Deaths, etc.)
        
        chronicleRows[0].UpdateRow(oldBest.MostKills, newBest.MostKills);         // Row for Kills
        chronicleRows[0].SetText("Kills");

        yield return new WaitForSeconds(0.5f);
        chronicleRows[1].UpdateRow(oldBest.MostDeath, newBest.MostDeath,true);         // Row for Deaths
        chronicleRows[1].SetText("Deaths");

        yield return new WaitForSeconds(0.5f);
        chronicleRows[2].UpdateRow(oldBest.KillToDeathRatio, newBest.KillToDeathRatio);  // Kill-to-death ratio
        chronicleRows[2].SetText("Kill/Death Ratio");

        yield return new WaitForSeconds(0.5f);
        chronicleRows[3].UpdateRow(oldBest.BestKillStreak, newBest.BestKillStreak);  // Best Kill Streak
        chronicleRows[3].SetText("Best Kill Streak");

        yield return new WaitForSeconds(0.5f);
        chronicleRows[4].UpdateRow(oldBest.TotalPlayTime, newBest.TotalPlayTime);  // Total Play Time
        chronicleRows[4].SetText("Total Play Time");

        yield return new WaitForSeconds(0.5f);
        chronicleRows[5].UpdateRow(oldBest.HighScore, newBest.HighScore);          // High Score
        chronicleRows[5].SetText("High Score");
    }
}
