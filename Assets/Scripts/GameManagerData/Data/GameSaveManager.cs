using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }

    private string playerDataPath = "";
    private string currentChronicleDataPath = "";
    private string completedChronicleDataPath = "";
    private string xpDataPath = "";
    private string abilityDataPath = "";
    
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
        
        playerDataPath = Application.persistentDataPath + "/playerData.json";
        currentChronicleDataPath = Application.persistentDataPath + "/currentChronicleSave.json";
        completedChronicleDataPath = Application.persistentDataPath + "/completedChroniclesSave.json";
        xpDataPath = Application.persistentDataPath + "/xpData.json";
        abilityDataPath = Application.persistentDataPath + "/abilityData.json";  // Path to save ability data

    }
    
    public void SaveXPData()
    {
        XPSaveData xpData = new XPSaveData(
            XPManager.Instance.GetCurrentLevel(),
            XPManager.Instance.GetCurrentXP(),
            XPManager.Instance.GetXPToNextLevel()
        );

        string jsonData = JsonUtility.ToJson(xpData, true);
        File.WriteAllText(xpDataPath, jsonData);

        Debug.Log("XP data saved successfully.");
    }
    
    public void ClearXPData()
    {
        XPSaveData xpData = new XPSaveData(
            1,
            0,
            0
        );

        string jsonData = JsonUtility.ToJson(xpData, true);
        File.WriteAllText(xpDataPath, jsonData);

        //Debug.Log("XP data saved successfully.");
    }
    public bool LoadXPData()
    {
        if (File.Exists(xpDataPath))
        {
            try
            {
                string jsonData = File.ReadAllText(xpDataPath);
                XPSaveData loadedData = JsonUtility.FromJson<XPSaveData>(jsonData);

                // Set the values back to the XPManager
                XPManager.Instance.SetCurrentLevel(loadedData.currentLevel);
                XPManager.Instance.SetCurrentXP(loadedData.currentXP);
                XPManager.Instance.SetXPToNextLevel(loadedData.xpToNextLevel);

                //Debug.Log("XP data loaded successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading XP data: {ex.Message}");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("No XP data found.");
            return false;
        }
    }
    // Save Player Data to JSON
    public void SavePlayerData(PlayerData playerData)
    {

        string jsonData = JsonUtility.ToJson(playerData, true); // Convert PlayerData to JSON
        File.WriteAllText(playerDataPath, jsonData); // Save to file
    }
    // Load Player Data from JSON
    public bool LoadPlayerData(out PlayerData loadedData)
    {
        // Check if the file exists
        if (File.Exists(playerDataPath))
        {
            try
            {
                string jsonData = File.ReadAllText(playerDataPath); // Read from file
                loadedData = JsonUtility.FromJson<PlayerData>(jsonData); // Convert JSON back to PlayerData
                return true; // Successfully loaded
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load player data: {ex.Message}");
                loadedData = new PlayerData();  // Provide a new instance of PlayerData to avoid null exceptions
                return false; // Indicate loading failure
            }
        }
        else
        {
            Debug.LogWarning("No player data found. Returning default data.");
            loadedData = new PlayerData();  // Provide a new instance of PlayerData to avoid null exceptions
            return false; // Indicate that there was no file
        }
    }
    
    // Save the current abilities to a JSON file
    public void SaveAbilityData(List<string> activeAbilities)
    {
        AbilitySaveData saveData = new AbilitySaveData { activeAbilities = activeAbilities };
        string json = JsonUtility.ToJson(saveData, true);  // Convert to JSON
        File.WriteAllText(abilityDataPath, json);  // Write to file

        Debug.Log("Ability data saved successfully.");
    }

    // Load ability data from a file
    public AbilitySaveData LoadAbilityData()
    {
        if (File.Exists(abilityDataPath))
        {
            string json = File.ReadAllText(abilityDataPath);
            AbilitySaveData saveData = JsonUtility.FromJson<AbilitySaveData>(json);
            //Debug.Log("Ability data loaded successfully.");
            return saveData;
        }
        else
        {
            Debug.LogWarning("No ability data found.");
            return null;
        }
    }

    public void ClearPlayerData()
    {
        GameSaveManager.Instance.SavePlayerData(new PlayerData());
    }
    
    // Clear the ability data file
    public void ClearAbilityData()
    {
        if (File.Exists(abilityDataPath))
        {
            File.Delete(abilityDataPath);
            Debug.Log("Ability data cleared successfully.");
        }
        else
        {
            Debug.LogWarning("No ability data found to clear.");
        }
    }
    #region CurrentChronicles

    public void SaveCurrentChronicles(List<ChronicleData> currentChronicles, int chronicleIndex)
    {
        /*if (currentChronicles == null || currentChronicles.Count == 0)
        {
            Debug.LogError("No chronicles to save.");
            return;
        }*/

        CurrentChronicleSaveData saveData = new CurrentChronicleSaveData(currentChronicles, chronicleIndex);
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(currentChronicleDataPath, json);
    }

    public void LoadCurrentChronicles(out List<ChronicleData> currentChronicles, out int chronicleIndex)
    {
        if (File.Exists(currentChronicleDataPath))
        {
            try
            {
                string json = File.ReadAllText(currentChronicleDataPath);
                CurrentChronicleSaveData saveData = JsonUtility.FromJson<CurrentChronicleSaveData>(json);
                currentChronicles = saveData.chronicles ?? new List<ChronicleData>(); // Ensure a new list is initialized

                chronicleIndex = currentChronicles.Last().ChronicleNumberIndex + 1;
            }
            catch (Exception ex)
            {
                //Debug.LogError($"Error loading current chronicles: {ex.Message}");
                currentChronicles = new List<ChronicleData>();  // Provide a new instance to avoid null exceptions
                chronicleIndex = 0;
            }
        }
        else
        {
            currentChronicles = new List<ChronicleData>();  // Initialize the list if no data exists
            chronicleIndex = 0;
        }
    }

    #endregion

    #region CompletedChronicles

    public void SaveCompletedChronicles(List<ChronicleData> completedChronicles)
    {
        CompletedChronicleSaveData saveData = new CompletedChronicleSaveData(completedChronicles);
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(completedChronicleDataPath, json);
    }

    public List<ChronicleData> LoadCompletedChronicleHistory()
    {
        if (File.Exists(completedChronicleDataPath))
        {
            try
            {
                string json = File.ReadAllText(completedChronicleDataPath);
                CompletedChronicleSaveData saveData = JsonUtility.FromJson<CompletedChronicleSaveData>(json);
                return saveData.completedChronicles ?? new List<ChronicleData>(); // Ensure a new list is initialized
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading completed chronicles: {ex.Message}");
                return new List<ChronicleData>();  // Provide a new instance to avoid null exceptions
            }
        }
        return new List<ChronicleData>();  // Return an empty list if no save exists
    }

    #endregion

    #region SaveDataClasses

    [System.Serializable]
    public class CurrentChronicleSaveData
    {
        public List<ChronicleData> chronicles;
        public int chronicleIndex;

        public CurrentChronicleSaveData(List<ChronicleData> chronicles, int chronicleIndex)
        {
            this.chronicles = chronicles;
            this.chronicleIndex = chronicleIndex;
        }
    }

    [System.Serializable]
    public class CompletedChronicleSaveData
    {
        public List<ChronicleData> completedChronicles;

        public CompletedChronicleSaveData(List<ChronicleData> completedChronicles)
        {
            this.completedChronicles = completedChronicles;
        }
    }

    #endregion
    
    #region AbilitySaveData
    
    [System.Serializable]
    public class AbilitySaveData
    {
        public List<string> activeAbilities;  // List of active ability class names
    }
    
    #endregion AbilitySaveData
}
