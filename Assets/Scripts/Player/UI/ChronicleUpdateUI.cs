using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChronicleUpdateUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private UIExtension scoreText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI status;
    [SerializeField] private GameObject doneGreen;
    
    public void UpdateChronicleMainMenu(ChronicleData chronicleData)
    {
        var index = chronicleData.ChronicleNumberIndex + 1;
        titleText.text = "Chronicle\n" + "- " + index + " -";
        scoreText.LerpToNumber(chronicleData.HighScore);
        rankText.text = DifficultyManager.Instance.GetLetterRank(chronicleData.Rank);
        status.text = "Completed";
        doneGreen.SetActive(true);
    }

    public void SetStatusText()
    {
        status.text = "Waiting";
        doneGreen.SetActive(false);
    }
    
    public void ResetChronicles(int index)
    {
        index++;
        titleText.text = "Chronicle\n" + "- " + index + " -";
        scoreText.TextPro.text = 0.ToString();
        rankText.text = "N";
        status.text = index == 1 ? "Waiting" : "Locked";
        doneGreen.SetActive(false);
    }
}
