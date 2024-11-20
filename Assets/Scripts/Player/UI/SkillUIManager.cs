using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI availablePointsText;  // Text displaying available points
    [SerializeField] private TextMeshProUGUI costText;  // Text displaying available points
    [SerializeField] private UIExtension skillmenuOn; 
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button onButton;
    [SerializeField] private bool menuIsOpen;

    [Header("Skill Rows")]
    [SerializeField] private List<SkillRowUI> skillRows = new List<SkillRowUI>();  // List of skill row UIs (one for each skill)
    
    private Dictionary<SkillRowUI, int> initialPipValues = new Dictionary<SkillRowUI, int>();  // Track initial pip values for reset

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
    }

    private void Start()
    {
        // Set up listeners for Accept and Reset buttons
        acceptButton.onClick.AddListener(OnAccept);
        resetButton.onClick.AddListener(OnReset);
        onButton.onClick.AddListener(OpenSkillMenu);
    }

    public void UpdateRowsOfChildren()
    {
        foreach (var skillRowsUI in skillRows)
        {
            skillRowsUI.UpdateUI();
        }
    }

    public void IfSpendPointsOpenSkillUI()
    {
        if (PlayerReferenceManager.Instance.playerStatSystem.StatBuyingPlayerData.CanAffordToBuy())
        {
            OpenSkillMenu();
        }
    }

    public void OpenSkillMenu()
    {
        if (menuIsOpen) return;
        menuIsOpen = true;
        // Initialize skill rows and track initial pip values
        //FirebaseEventManager.Instance.LogSkillsScreenEvent();
        StartCoroutine(CoroutineOpenSkillMenu());
    }

    private IEnumerator CoroutineOpenSkillMenu()
    {
        PlayerReferenceManager.Instance.SetPlayerInMenus(true,"SkillUI");
        yield return new WaitForSeconds(0.5f);

        PlayerReferenceManager.Instance.playerStatSystem.StartPointBuy();
        skillmenuOn.FadeIn();
        InitializeSkillRows();
        UpdateAvailablePointsUI();
        UpdateRowsOfChildren();
        UIAudioManager.Instance.PlayGoodEvent();
    }

    public void CloseSkillMenu()
    {
        menuIsOpen = false;
        skillmenuOn.FadeOut();
        PlayerUI.Instance.SpendPointsUI();
        PlayerReferenceManager.Instance.SetPlayerInMenus(false);
        
    }

    private void InitializeSkillRows()
    {
        foreach (var skillRow in skillRows)
        {
            skillRow.OnPipChanged += UpdateAvailablePointsUI;  // Listen for changes in pip count
            skillRow.Initialise();
        }
    }

    // Update the available points display
    public void UpdateAvailablePointsUI()
    {
        availablePointsText.text = "Available Points: " + PlayerReferenceManager.Instance.playerStatSystem.StatBuyingPlayerData.CalculateSpentAvailablePoints();
        costText.text = "Cost: " + PlayerReferenceManager.Instance.playerStatSystem.StatBuyingPlayerData.CalculateCostStatBuy();
    }
    

    // When "Accept" is pressed
    private void OnAccept()
    {
        
        PlayerReferenceManager.Instance.playerStatSystem.UpdateAvailablePoints();
        UpdateAvailablePointsUI();
        PlayerReferenceManager.Instance.playerStatSystem.SetCurrentSkillPoints();
        CloseSkillMenu();
    }

    // When "Reset" is pressed
    private void OnReset()
    {
        PlayerReferenceManager.Instance.playerStatSystem.StartPointBuy();
        UpdateRowsOfChildren();
        UpdateAvailablePointsUI();  // Update UI to reflect the reset
    }
}
