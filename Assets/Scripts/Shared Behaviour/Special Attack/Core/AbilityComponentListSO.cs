using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewAbilityComponentList", menuName = "Abilities/AbilityComponentList")]
public class AbilityComponentListSO : ScriptableObject
{
    [Tooltip("List of component class names to add or remove dynamically")]
    public List<string> componentClassNames;  // Store the class names of the components

    // Add all components to the target GameObject
    public void AddComponents(GameObject target)
    {
        foreach (string className in componentClassNames)
        {
            Type componentType = Type.GetType(className);
            if (componentType != null && target.GetComponent(componentType) == null)
            {
                target.AddComponent(componentType);
                Debug.Log($"{className} added to {target.name}");
            }
        }
    }

    // Remove all components from the target GameObject
    public void RemoveComponents(GameObject target)
    {
        foreach (string className in componentClassNames)
        {
            Type componentType = Type.GetType(className);
            Component component = target.GetComponent(componentType);
            if (component != null)
            {
                Destroy(component);
                Debug.Log($"{className} removed from {target.name}");
            }
        }
    }
    
}