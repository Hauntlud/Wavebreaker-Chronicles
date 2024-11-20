using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance { get; private set; }
    
    [SerializeField] private List<ChronicleUpdateUI> chronicleUpdateUI;
    [SerializeField] private UpdateScores updateScores;
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
    }

    public void UpdateChroniclesUI(List<ChronicleData> chronicleDatas)
    {
        //print("List Count " + chronicleDatas.Count);
        
        for (int i = 0; i < chronicleDatas.Count; i++)
        {
            chronicleUpdateUI[i].UpdateChronicleMainMenu(chronicleDatas[i]);
        }

        if (chronicleDatas.Count != 4)
        {
            chronicleUpdateUI[chronicleDatas.Count].SetStatusText();
        }
        
    }

    public void UpdateHighScores()
    {
        updateScores.UpdateScoreUI(GameDataManager.Instance.GetBestFromSavedChronicles());
    }

    
    public void ResetChronicles()
    {
        for (int i = 0; i < 4; i++)
        {
            chronicleUpdateUI[i].ResetChronicles(i);
        }
    }
}
