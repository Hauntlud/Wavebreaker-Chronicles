using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveSpawnerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentWaveText;
    [SerializeField] private TextMeshProUGUI currentRank;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private UIExtension shakeUI;
    
    public void UpdateUI(int currentWave, int maxWave)
    {
        currentWaveText.text = currentWave + " / " + maxWave;
        shakeUI.ShakeUIElement();
    }

    public void UpdateWaveSkill()
    {
        currentRank.text = DifficultyManager.Instance.GetLetterRank(DifficultyManager.Instance.GetCurrentNormDifficulty());
    }

    public void UpdateTimer(float timer)
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        string timerString = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = timerString;
    }


}
