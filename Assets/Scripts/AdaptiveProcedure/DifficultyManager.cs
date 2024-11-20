using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Difficulty Settings")]
    [SerializeField] private AdaptiveProcedureSettings firstChronicleDifficulty;
    [SerializeField] private AdaptiveProcedureSettings secondChronicleDifficulty;
    [SerializeField] private AdaptiveProcedureSettings thirdChronicleDifficulty;
    [SerializeField] private AdaptiveProcedureSettings fourthChronicleDifficulty;

    // List to hold multiple adaptive procedures
    [SerializeField] private List<AdaptiveProcedure> adaptiveProcedures = new List<AdaptiveProcedure>();
    [SerializeField] private AdaptiveProcedure killDeathProcedure;
    [SerializeField] private AdaptiveProcedureSettings currentSettings;
    
    [SerializeField] private float difficulty;
    [SerializeField] private float normDifficulty;

    private void Awake()
    {
        transform.parent = null;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        AddAdaptiveProcedure(killDeathProcedure);

    }
    private void Start()
    {
        AdjustDifficulty();
    }

    public void UpdateDifficulty(float difficultyIn)
    {
        foreach (var adaptiveProcedure in adaptiveProcedures)
        {
            adaptiveProcedure.SetDifficulty(difficultyIn);
        }
        
        //print("Set Difficulty " + difficultyIn);
        difficulty = difficultyIn;

        GetCurrentDifficulty();
        GetCurrentNormDifficulty();
    }
    

    // Method to set the difficulty and apply the appropriate ScriptableObject to all adaptive procedures
    public void SetSettingsDifficulty(AdaptiveProcedureSettings difficultyLevel)
    {
        currentSettings = difficultyLevel;

        foreach (var adaptiveProcedure in adaptiveProcedures)
        {
            adaptiveProcedure.SetSettings(currentSettings);
            adaptiveProcedure.InitializeProcedure();
        }
    }

    // Method to add adaptive procedures to the manager's list
    public void AddAdaptiveProcedure(AdaptiveProcedure adaptiveProcedure)
    {
        if (!adaptiveProcedures.Contains(adaptiveProcedure))
        {
            adaptiveProcedures.Add(adaptiveProcedure);
        }
    }

    // Method to process a response (correct/incorrect) and apply it to all adaptive procedures
    public void ProcessKillResponse (bool isCorrect)
    {
        killDeathProcedure.RegisterResponse(isCorrect);
    }

    // Method to dynamically adjust difficulty mid-game (could be triggered by certain events)
    public void AdjustDifficulty()
    {
        var instanceCurrentChronicleIndex = GameDataManager.Instance.CurrentChronicleIndex;
        switch (instanceCurrentChronicleIndex)
        {
            case 0:
                SetSettingsDifficulty(firstChronicleDifficulty);
                break;
            case 1:
                SetSettingsDifficulty(secondChronicleDifficulty);
                break;
            case 2:
                SetSettingsDifficulty(thirdChronicleDifficulty);
                break;
            case 3:
                SetSettingsDifficulty(fourthChronicleDifficulty);
                break;
        }
        
    }

    // Method to get the average difficulty from all adaptive procedures
    public float GetCurrentDifficulty()
    {
        if (adaptiveProcedures.Count == 0)
        {
            Debug.LogWarning("No adaptive procedures available.");
            return 0f;  // Default if no adaptive procedures are present
        }

        // Calculate the average difficulty from all adaptive procedures
        float averageDifficulty = adaptiveProcedures.Average(procedure => procedure.GetCurrentDifficulty());
        difficulty = averageDifficulty;
        return averageDifficulty;
    }
    
    public float GetCurrentNormDifficulty()
    {
        if (adaptiveProcedures.Count == 0)
        {
            Debug.LogWarning("No adaptive procedures available.");
            return 0f;  // Default if no adaptive procedures are present
        }

        // Calculate the average difficulty from all adaptive procedures
        float averageNormDifficulty = adaptiveProcedures.Average(procedure => procedure.GetNormOfCurrentDifficulty());
        normDifficulty = averageNormDifficulty;
        return averageNormDifficulty;
    }
    
    public string GetLetterRank(float normalizedValue)
    {
        return normalizedValue switch
        {
            <= 0.1f => "F",
            <= 0.2f => "D-",
            <= 0.3f => "D",
            <= 0.4f => "D+",
            <= 0.5f => "C-",
            <= 0.6f => "C",
            <= 0.7f => "C+",
            <= 0.8f => "A-",
            <= 0.9f => "A",
            <= 0.95f => "A+",
            _ => "S"
        };
    }

    
    // Method to get the normalized value based on the rank
    public float GetNormalizedValueFromRank(string rank)
    {
        return rank switch
        {
            "F" => 0.1f,
            "D-" => 0.2f,
            "D" => 0.3f,
            "D+" => 0.4f,
            "C-" => 0.5f,
            "C" => 0.6f,
            "C+" => 0.7f,
            "A-" => 0.8f,
            "A" => 0.9f,
            "A+" => 0.95f,
            "S" => 1f,
            _ => 0f  // Default for unknown rank
        };
    }

    
}
