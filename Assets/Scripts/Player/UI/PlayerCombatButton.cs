using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatButton : MonoBehaviour
{
    public void ButtonPressed()
    {
        SwitchUI();
    }
    
    private void SwitchUI()
    {
        PlayerReferenceManager.Instance.playerCombatController.ChangeAttacks();
        PlayerUI.Instance.SwitchAttacks();
        
    }
}
