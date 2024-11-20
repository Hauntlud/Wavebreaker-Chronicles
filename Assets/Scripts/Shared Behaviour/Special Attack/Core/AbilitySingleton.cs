using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AbilitySingleton : MonoBehaviour
{
    public static AbilitySingleton Instance { get; private set; }

    [SerializeField] private List<SpecialAbilityData> abilities; // List of all ability ScriptableObjects
    private Dictionary<string, SpecialAbilityData> abilityDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAbilityDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAbilityDictionary()
    {
        abilityDictionary = new Dictionary<string, SpecialAbilityData>();
        foreach (var ability in abilities)
        {
            if (ability != null && !abilityDictionary.ContainsKey(ability.abilityName))
            {
                abilityDictionary[ability.abilityName] = ability;
            }
        }
    }

    public SpecialAbilityData GetAbilityData(string abilityName)
    {
        abilityDictionary.TryGetValue(abilityName, out SpecialAbilityData abilityData);
        return abilityData;
    }

    [Button("Populate Ability List")]
    private void GetAbilityNames()
    {
#if UNITY_EDITOR
        abilities.Clear(); // Clear the current list

        // Find all assets of type SpecialAbilityData in the project
        string[] guids = AssetDatabase.FindAssets("t:SpecialAbilityData");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SpecialAbilityData abilityData = AssetDatabase.LoadAssetAtPath<SpecialAbilityData>(path);
            if (abilityData != null)
            {
                abilities.Add(abilityData); // Add to the list
            }
        }

        // Mark the object as dirty to save changes
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        Debug.Log("Ability list populated with available SpecialAbilityData assets.");
#endif
    }
}
