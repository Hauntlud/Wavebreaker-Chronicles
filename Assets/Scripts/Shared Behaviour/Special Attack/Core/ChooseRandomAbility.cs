using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChooseRandomAbility : MonoBehaviour
{
    [SerializeField] private AbilityManager abilityManager;

    private void Start()
    {
        AbilityManager();
    }

    public void AbilityManager()
    {
        abilityManager.AddRandomAbilityComponent();
    }
}
