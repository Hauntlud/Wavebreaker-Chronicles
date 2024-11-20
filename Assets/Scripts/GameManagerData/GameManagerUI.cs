using System.Collections;
using NaughtyAttributes;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour
{
    public static GameManagerUI Instance { get; private set; }
    
    [SerializeField] private UIExtension mainMenu;
    [SerializeField] private UIExtension settingsMenu;
    [SerializeField] private Button startGame;
    
    [BoxGroup("Settings")] [SerializeField] private Button settingsButtonOn;
    [BoxGroup("Settings")] [SerializeField] private Button settingsButtonOnChronicleUI;
    [BoxGroup("Settings")] [SerializeField] private Button settingsButtonOnInGame;
    [BoxGroup("Settings")] [SerializeField] private Button settingsButtonOff;
    [BoxGroup("Settings")] [SerializeField] private Button settingQuitButton;
    
    
    [BoxGroup("Reset")] [SerializeField] private Button resetButton;
    
    [BoxGroup("End Chronicle Screen")] [SerializeField] private Button chronicleQuitButton;
    [BoxGroup("End Chronicle Screen")] [SerializeField] private Button chronicleContinueButton;
    [BoxGroup("End Chronicle Screen")] [SerializeField] private TextMeshProUGUI continueText;

    private bool playerInSettings;
    
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
        startGame.onClick.AddListener(StartGame);
        
        
        settingsButtonOn.onClick.AddListener(TurnOnSettings);
        settingsButtonOnInGame.onClick.AddListener(TurnOnSettings);
        settingsButtonOnChronicleUI.onClick.AddListener(TurnOnSettings);
        settingsButtonOff.onClick.AddListener(TurnOffSettings);
        resetButton.onClick.AddListener(ResetChronicles);
        
        settingQuitButton.onClick.AddListener(QuitGame);
        
        chronicleQuitButton.onClick.AddListener(QuitGame);
        chronicleContinueButton.onClick.AddListener(ContinueChronicle);
        
    }

    private void StartGame()
    {
        ToggleUIMenu(false);
        GameManager.Instance.StartGame();
    }

    public void ResetChronicles()
    {
        GameManager.Instance.ResetChronicles();
    }

    public void ToggleUIMenu(bool toggle)
    {
        if (toggle)
        {
            mainMenu.FadeIn();
            FirebaseEventManager.Instance.LogMainMenuEvent();
            PlayerReferenceManager.Instance.SetPlayerInMenus(true,"MainMenu");
            MainMenuUI.Instance.UpdateChroniclesUI(GameDataManager.Instance.CurrentChronicleGameData);
            MainMenuUI.Instance.UpdateHighScores();
            if (GameDataManager.Instance.CurrentChronicleIndex == 0)
            {
                continueText.text = "Start\nChronicles";
            }
            else
            {
                continueText.text = "Continue\nChronicles";
            }

        }
        else
        {
            PlayerReferenceManager.Instance.SetPlayerInMenus(false);
            mainMenu.FadeOut();
        }
    }

    public void TurnOnSettings()
    {
        if (playerInSettings) return;
        playerInSettings = true;
        PlayerReferenceManager.Instance.SetPlayerInMenus(true,"Settings");
        StartCoroutine(CoroutineTurnOnSettings());
    }

    private IEnumerator CoroutineTurnOnSettings()
    {
        yield return new WaitForSeconds(0.5f);
        FirebaseEventManager.Instance.LogSettingsMenuEvent();
        settingsMenu.transform.localScale = Vector3.one;
        settingsMenu.FadeIn();
    }

    public void TurnOffSettings()
    {
        playerInSettings = false;
        PlayerReferenceManager.Instance.SetPlayerInMenus(false);
        settingsMenu.transform.localScale = Vector3.one;
        settingsMenu.FadeOut();
    }
    
    public void QuitGame()
    {
        FirebaseEventManager.Instance.LogGameQuitEvent();
        Application.Quit();
#if UNITY_EDITOR
        // Check if the application is running in the editor
        if (Application.isEditor)
        {
            // Stop the editor from playing
            EditorApplication.isPlaying = false;
        }
#endif
    }
    
    public void ContinueChronicle()
    {
        GameManager.Instance.NextChronicle();
    }
    
}
