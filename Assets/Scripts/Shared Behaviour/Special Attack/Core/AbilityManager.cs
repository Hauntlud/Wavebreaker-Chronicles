using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [SerializeField] private AbilityComponentListSO abilityComponentList;  // Reference to the ScriptableObject with ability components
    [SerializeField] private List<string> activeAbilities = new List<string>();  // Track currently active ability class names

    public void TriggerPlayerGetAbility()
    {
        AbilityUIManager.Instance.OpenAbilityMenu();
    }

    // Check if a specific ability is already active
    public bool HasAbility(string className)
    {
        Type componentType = Type.GetType(className);
        return componentType != null && gameObject.GetComponent(componentType) != null;
    }
    
    public List<string> GetActiveAbilityData()
    {
        return new List<string>(activeAbilities); // Return a copy of the active abilities list
    }

    // Get a list of active ability names for saving or UI purposes
    public List<string> GetActiveAbilityNames()
    {
        return new List<string>(activeAbilities);
    }

    // Get a random ability that the player does not already have
    public string GetRandomAbility()
    {
        List<string> availableAbilities = new List<string>();

        // Filter out abilities that are already active
        foreach (string className in abilityComponentList.componentClassNames)
        {
            if (!activeAbilities.Contains(className))
            {
                availableAbilities.Add(className);
            }
        }

        // Return a random ability if there are any available
        if (availableAbilities.Count > 0)
        {
            return availableAbilities[UnityEngine.Random.Range(0, availableAbilities.Count)];
        }

        Debug.LogWarning("No available abilities left to select randomly.");
        return null;
    }

    // Add a specific ability by class name
    public void AddAbility(string className)
    {
        Type componentType = Type.GetType(className);
        if (componentType != null && gameObject.GetComponent(componentType) == null)
        {
            gameObject.AddComponent(componentType);
            if (!activeAbilities.Contains(className))
            {
                activeAbilities.Add(className);
                //Debug.Log($"Added ability: {className}");
            }
        }
        else
        {
            Debug.LogWarning($"Ability {className} is already active or type is invalid.");
        }
    }

    // Save ability data using the GameSaveManager
    public void SaveAbilityData()
    {
        GameSaveManager.Instance.SaveAbilityData(activeAbilities);
    }

    // Load ability data using the GameSaveManager
    public void LoadAbilityData()
    {
        GameSaveManager.AbilitySaveData loadedData = GameSaveManager.Instance.LoadAbilityData();
        if (loadedData != null)
        {
            RemoveAbilityComponents();
            foreach (string className in loadedData.activeAbilities)
            {
                AddAbility(className);
            }
        }
        AbilityUIManager.Instance.UpdateAbilityUI();
    }
    
    public void AddRandomAbilityComponent()
    {
        if (abilityComponentList != null && gameObject != null && abilityComponentList.componentClassNames.Count > 0)
        {
            string randomAbilityClass = abilityComponentList.componentClassNames[UnityEngine.Random.Range(0, abilityComponentList.componentClassNames.Count)];
            Type componentType = Type.GetType(randomAbilityClass);

            if (componentType != null && gameObject.GetComponent(componentType) == null)
            {
                gameObject.AddComponent(componentType);
            }
        }
    }

    // Clear ability data using the GameSaveManager
    public void ClearAbilityData()
    {
        RemoveAbilityComponents();
        GameSaveManager.Instance.ClearAbilityData();
    }

    // Remove all components related to abilities
    public void RemoveAbilityComponents()
    {
        foreach (string className in activeAbilities)
        {
            Type componentType = Type.GetType(className);
            if (componentType != null)
            {
                Component component = gameObject.GetComponent(componentType);
                if (component != null)
                {
                    Destroy(component);
                }
            }
        }

        activeAbilities.Clear();
    }
}
