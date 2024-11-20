using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllChronicleEndRow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textToUpdate;
    
    [SerializeField] private UIExtension differenceText;
    [SerializeField] private UIExtension improvementText;
    
    
    [SerializeField] private Color improvementColor;
    [SerializeField] private Color sameColor;
    [SerializeField] private Color declineColor;



    
    public void SetText(string textIn)
    {
        textToUpdate.text = textIn;
    }
    
   public void UpdateRow(float currentNumber, float newNumber, bool minIsBest = false)
{
    float difference = currentNumber - newNumber;
    
    differenceText.TextPro.text = "0";  // Reset text
    differenceText.LerpToNumber(difference);  // Visual animation of the difference
    
    if (minIsBest)  // If lower numbers are better (e.g., deaths)
    {
        // In this case, a negative difference (newNumber < currentNumber) is better
        if (difference > 0)  // Worse performance (newNumber is higher)
        {
            improvementText.TextPro.text = "Try Again";
            improvementText.TextPro.color = declineColor;
            differenceText.TextPro.color = declineColor;
        }
        else if (difference == 0)  // Same performance
        {
            improvementText.TextPro.text = "Acceptable";
            improvementText.TextPro.color = sameColor;
            differenceText.TextPro.color = sameColor;
        }
        else  // Better performance (newNumber is lower)
        {
            improvementText.TextPro.text = "Exceptional";
            improvementText.TextPro.color = improvementColor;
            differenceText.TextPro.color = improvementColor;
        }
    }
    else  // If higher numbers are better (e.g., kills, score)
    {
        // In this case, a positive difference (newNumber > currentNumber) is better
        if (difference > 0)  // Better performance
        {
            improvementText.TextPro.text = "Exceptional";
            improvementText.TextPro.color = improvementColor;
            differenceText.TextPro.color = improvementColor;
        }
        else if (difference == 0)  // Same performance
        {
            improvementText.TextPro.text = "Acceptable";
            improvementText.TextPro.color = sameColor;
            differenceText.TextPro.color = sameColor;
        }
        else  // Worse performance
        {
            improvementText.TextPro.text = "Try Better";
            improvementText.TextPro.color = declineColor;
            differenceText.TextPro.color = declineColor;
        }
    }
}

}
