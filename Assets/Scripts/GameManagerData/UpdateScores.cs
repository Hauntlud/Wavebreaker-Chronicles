using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateScores : MonoBehaviour
{
    [SerializeField] private UIExtension mostKills;
    [SerializeField] private UIExtension bestKillStreak;
    [SerializeField] private UIExtension killToDeathRatio;
    [SerializeField] private UIExtension totalPlaytime;
    [SerializeField] private UIExtension averagePerformance;
    [SerializeField] private UIExtension highscore;

    public void UpdateScoreUI(ChronicleData chronicleData)
    {
        StartCoroutine(TextCounter(chronicleData));
    }

    IEnumerator TextCounter(ChronicleData chronicleData)
    {
        
        yield return new WaitForSeconds (0.25f);
        
        if (mostKills != null)
        {
            mostKills.LerpToNumber(chronicleData.MostKills);
        }

        yield return new WaitForSeconds (0.25f);
        
        if (bestKillStreak != null)
        {
            bestKillStreak.LerpToNumber(chronicleData.BestKillStreak);
        }
        
        yield return new WaitForSeconds (0.25f);
        
        if (killToDeathRatio != null)
        {
            killToDeathRatio.LerpToNumber(chronicleData.KillToDeathRatio);
        }
        
        yield return new WaitForSeconds (0.25f);
        
        if (totalPlaytime != null)
        {
            var mins = chronicleData.TotalPlayTime / 60;
            totalPlaytime.TextPro.text = mins.ToString("F2") + " mins" ;
        }
        
        yield return new WaitForSeconds (0.25f);
        
        if (averagePerformance != null)
        {
            averagePerformance.TextPro.text = DifficultyManager.Instance.GetLetterRank(chronicleData.Rank);
        }
        
        yield return new WaitForSeconds (0.25f);
        
        if (highscore != null)
        {
            highscore.LerpToNumber(chronicleData.HighScore);
        }
    }
}
