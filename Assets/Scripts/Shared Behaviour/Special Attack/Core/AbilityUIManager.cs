using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIManager : MonoBehaviour
{
    public static AbilityUIManager Instance { get; private set; }
    
    [SerializeField] private UIExtension uiExtension;
    [SerializeField] private List<Image> currentAbilityImages;
    [SerializeField] private List<TextMeshProUGUI> currentAbilityNames;
    [SerializeField] private List<Image> activeAbilities;
    [SerializeField] private List<Sprite> numbers;

    [SerializeField] private Image newAbilityImage;
    [SerializeField] private TextMeshProUGUI newAbilityName;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button tryAgainButton;

    private string newAbilityClassName;
    private bool isOpen;

    private void Awake()
    {
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
        acceptButton.onClick.AddListener(OnAcceptButtonClick);
        tryAgainButton.onClick.AddListener(OnTryAgainButtonClick);
    }

    public void OpenAbilityMenu()
    {
        if (isOpen) return;
        if (PlayerReferenceManager.Instance.playerAbilityManager.GetActiveAbilityData().Count >= 3) return;
        tryAgainButton.interactable = true;
        PlayerReferenceManager.Instance.SetPlayerInMenus(true,"OpenAbilityMenu");
        uiExtension.FadeIn();
        DisplayCurrentAbilities();
        ShowNewAbility();
        isOpen = true;
    }

    // Populate the UI with the player's current abilities
    private void DisplayCurrentAbilities()
    {
        List<string> activeAbilityNames = PlayerReferenceManager.Instance.playerAbilityManager.GetActiveAbilityData();

        for (int i = 0; i < currentAbilityImages.Count; i++)
        {
            if (i < activeAbilityNames.Count)
            {
                SpecialAbilityData abilityData = AbilitySingleton.Instance.GetAbilityData(activeAbilityNames[i]);
                
                if (abilityData != null)
                {
                    currentAbilityImages[i].sprite = abilityData.spriteImage;
                    currentAbilityNames[i].text = abilityData.abilityNameString;
                    currentAbilityImages[i].gameObject.SetActive(true);
                    currentAbilityNames[i].gameObject.SetActive(true);
                }
            }
            else
            {
                currentAbilityImages[i].sprite = numbers[i];
                currentAbilityNames[i].gameObject.SetActive(false);
            }
        }
    }

    // Show a new random ability
    private void ShowNewAbility()
    {
        string previousAbilityClassName = newAbilityClassName;
        newAbilityClassName = PlayerReferenceManager.Instance.playerAbilityManager.GetRandomAbility();

        // Reroll if the selected ability is the same as the previous one
        int attempts = 0;
        while (!string.IsNullOrEmpty(newAbilityClassName) && newAbilityClassName == previousAbilityClassName && attempts < 5)
        {
            newAbilityClassName = PlayerReferenceManager.Instance.playerAbilityManager.GetRandomAbility();
            attempts++;
        }

        if (!string.IsNullOrEmpty(newAbilityClassName))
        {
            SpecialAbilityData abilityData = AbilitySingleton.Instance.GetAbilityData(newAbilityClassName);
            if (abilityData != null)
            {
                newAbilityImage.sprite = abilityData.spriteImage;
                newAbilityName.text = abilityData.abilityNameString;
                newAbilityImage.gameObject.SetActive(true);
                newAbilityName.gameObject.SetActive(true);
                acceptButton.gameObject.SetActive(true);
                tryAgainButton.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"Ability data not found for {newAbilityClassName}");
            }
        }
        else
        {
            Debug.LogWarning("No new ability to show!");
        }
    }

    // Accept the new ability and update the UI
    public void OnAcceptButtonClick()
    {
        if (!string.IsNullOrEmpty(newAbilityClassName))
        {
            PlayerReferenceManager.Instance.playerAbilityManager.AddAbility(newAbilityClassName);
            DisplayCurrentAbilities();
            UpdateAbilityUI();

            // Hide the new ability UI
            newAbilityImage.gameObject.SetActive(false);
            newAbilityName.gameObject.SetActive(false);
            acceptButton.gameObject.SetActive(false);
            tryAgainButton.gameObject.SetActive(false);

            StartCoroutine(CloseAfterSomeTime());
        }
    }

    // Update the ability UI to reflect currently active abilities
    public void UpdateAbilityUI()
    {
        List<string> activeAbilityNames = PlayerReferenceManager.Instance.playerAbilityManager.GetActiveAbilityData();

        for (int i = 0; i < activeAbilities.Count; i++)
        {
            if (i < activeAbilityNames.Count)
            {
                SpecialAbilityData abilityData = AbilitySingleton.Instance.GetAbilityData(activeAbilityNames[i]);
                
                if (abilityData != null)
                {
                    activeAbilities[i].sprite = abilityData.spriteImage;
                    activeAbilities[i].gameObject.SetActive(true);
                }
            }
            else
            {
                activeAbilities[i].gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator CloseAfterSomeTime()
    {
        yield return new WaitForSeconds(0.25f);
        PlayerReferenceManager.Instance.SetPlayerInMenus(false);
        uiExtension.FadeOut();
        isOpen = false;
    }

    // Reroll for a new ability
    public void OnTryAgainButtonClick()
    {
        ShowNewAbility();
        tryAgainButton.interactable = false;
    }
}
