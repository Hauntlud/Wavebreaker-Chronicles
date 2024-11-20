using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector3 = System.Numerics.Vector3;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }  // Singleton instance
    [SerializeField] private GameManagerUI gameManagerUI; 
    [SerializeField] private ObjectSpawner objectSpawner; 
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private WaveSpawner waveSpawner;
    
    [SerializeField] private float timeScaleStep = 0.25f;  // Step size for changing the speed
    [SerializeField] private float minTimeScale = 0.25f;   // Minimum allowed time scale
    [SerializeField] private float maxTimeScale = 15;     // Maximum allowed time scale

    void Update()
    {
        // This code will only run in the Unity Editor
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftBracket))  // Detect '[' key press
        {
            ChangeTimeScale(-timeScaleStep);  // Slow down the game
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))  // Detect ']' key press
        {
            ChangeTimeScale(timeScaleStep);  // Speed up the game
        }

        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            SetTimeScale(1);
        }
        #endif
    }

    // Method to change the time scale, ensuring it stays within a reasonable range
    private void ChangeTimeScale(float adjustment)
    {
        Time.timeScale = Mathf.Clamp(Time.timeScale + adjustment, minTimeScale, maxTimeScale);
        Debug.Log("New Time Scale: " + Time.timeScale);
    }

    private void SetTimeScale(float newSet)
    {
        Time.timeScale = newSet;
    }
    
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
    private void Start()
    {
        InitialiseManagers();
        //FirebaseEventManager.Instance.LogGameOpenEvent();
    }
    
    
    public void InitialiseManagers()
    {
        XPManager.Instance.Initialize();

        
        gameManagerUI.ToggleUIMenu(true);

    }

    public void StartGame()
    {
        EventSystem.current.SetSelectedGameObject(null);
        FirebaseEventManager.Instance.LogGameStartEvent();
        StartCoroutine(StartGameCoroutine());
        
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        UpdateDifficulty();
        waveSpawner.ResetWaves();
        playerSpawner.SpawnPlayer(new UnityEngine.Vector3(0,0,0));
        
        MusicManager.Instance.PlayRandomMusic();
        waveSpawner.TurnOn();
        objectSpawner.StartChronicleSpawning();
    }

    public void EndChronicle(string waveStatus)
    {
        UIAudioManager.Instance.PlayGoodEvent();
        
        GameDataManager.Instance.SetScore();
        GameDataManager.Instance.SetRank();
        GameDataManager.Instance.SetBestKillStreak();
        GameDataManager.Instance.SetKillsToDeath();
        GameDataManager.Instance.SetNumber();
        GameDataManager.Instance.SetDifficulty();
        
        GameDataManager.Instance.AddChronicleToCurrentChronicleData();
        GameDataManager.Instance.SaveGameData();
        
        PlayerReferenceManager.Instance.playerAbilityManager.SaveAbilityData();
        
        PlayerReferenceManager.Instance.playerStatSystem.SetCurrentSkillPoints();
        PlayerReferenceManager.Instance.playerStatSystem.SavePlayerData();
        GameSaveManager.Instance.SaveXPData();
        
        
        if (GameDataManager.Instance.CurrentChronicleIndex == 3)
        {
            NextChronicle();
        }
        else
        {
            ChronicleEndUI.Instance.OpenEndChronicleUI(waveStatus);
        }
        
        
    }

    public void ResetChronicles()
    {
        print("Waves Resetting");
        GameDataManager.Instance.ClearChronicles();
        MainMenuUI.Instance.ResetChronicles();
        GameSaveManager.Instance.ClearXPData();
        GameSaveManager.Instance.LoadXPData();
        GameSaveManager.Instance.ClearPlayerData();
        
        if (PlayerReferenceManager.Instance.playerAbilityManager != null)
        {
            PlayerReferenceManager.Instance.playerAbilityManager.ClearAbilityData();
            playerSpawner.InitialisePlayer();
            AbilityUIManager.Instance.UpdateAbilityUI();
        }
        
        GameSaveManager.Instance.ClearAbilityData();
        waveSpawner.ResetWaves();
        PlayerUI.Instance.SetScore(0);

    }

    public void NextChronicle()
    {

        ChronicleEndUI.Instance.CloseEndChronicleUI();
        
        if (GameDataManager.Instance.CurrentChronicleIndex == 3)
        {
            AllChroniclesEnd.Instance.UpdateAllChronicles();
            GameDataManager.Instance.ClearChronicles();
            GameSaveManager.Instance.ClearXPData();
            GameSaveManager.Instance.ClearAbilityData();
            GameDataManager.Instance.StartNewChronicle();
        }
        else
        {
            GameDataManager.Instance.StartNewChronicle();
            UpdateDifficulty();

            waveSpawner.ProceedToNextChronicle();
            objectSpawner.StartChronicleSpawning();
            
        }
    }

    private static void UpdateDifficulty()
    {
        DifficultyManager.Instance.AdjustDifficulty();

        if (GameDataManager.Instance.CurrentChronicleGameData.Count != 0)
        {
            //print("Saved Diff " + GameDataManager.Instance.CurrentChronicleGameData.Last().Difficulty);
            DifficultyManager.Instance.UpdateDifficulty(GameDataManager.Instance.CurrentChronicleGameData.Last().Difficulty);
        }
        else
        {
            DifficultyManager.Instance.UpdateDifficulty(0);
        }
        
        
    }
}