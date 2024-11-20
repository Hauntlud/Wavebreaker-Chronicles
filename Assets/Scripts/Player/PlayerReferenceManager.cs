using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PlayerReferenceManager : MonoBehaviour
{
    public static PlayerReferenceManager Instance { get; private set; }

    public Transform PlayerTransform { get; private set; }

    public Transform playerTransformRef;
    public PlayerController playerController;
    public PlayerStatSystem playerStatSystem;
    public PlayerCombatController playerCombatController;
    public PlayerMovementController PlayerMovementController;
    public AbilityManager playerAbilityManager;
    
    [SerializeField] private bool isForceMobile;
    private string previousMenu = "";
    
    private bool isMobile;
    [SerializeField] private bool playerInMenus;
    public bool PlayerInMenus => playerInMenus;
    public bool IsMobile => isMobile;
    [SerializeField] private int playerInMenusInt;

    private void Awake()
    {
        transform.parent = null;
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
        isMobile = Application.isMobilePlatform || isForceMobile;
        
    }

    private void Update()
    {
        IsPointerOverUIElementComputer();
    }

    public void SetPlayerInMenus(bool toggle,string menuOpenString = "")
    {
        if (toggle && menuOpenString != previousMenu)
        {
            playerInMenusInt++;
        }
        else
        {
            playerInMenusInt = Mathf.Clamp(playerInMenusInt - 1,0,99);
        }
        playerInMenus = playerInMenusInt >= 1;
        previousMenu = menuOpenString;
    }

    public void ResetPlayerInMenus()
    {
        playerInMenusInt = 0;
        playerInMenus = false;
    }

    public void SetPlayerReference(Transform playerTransform)
    {
        PlayerTransform = playerTransform;
        playerTransformRef = PlayerTransform;
        playerController = PlayerTransform.GetComponent<PlayerController>();
        playerCombatController = PlayerTransform.GetComponent<PlayerCombatController>();
        PlayerMovementController = PlayerTransform.GetComponent<PlayerMovementController>();
        playerStatSystem = PlayerTransform.GetComponent<PlayerStatSystem>();
        playerAbilityManager = PlayerTransform.GetComponent<AbilityManager>();
    }
    
    public bool IsPointerOverUIElementComputer()
    {
        // On PC, check if the mouse is over a UI element
        //print("Pointer over UI " + EventSystem.current.IsPointerOverGameObject());
        return EventSystem.current.IsPointerOverGameObject();
    }
    
    public bool IsPointerOverUIElementMobile(Touch touch)
    {
        return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }
}