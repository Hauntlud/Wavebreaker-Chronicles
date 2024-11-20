using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChronicleEndUI : MonoBehaviour
{
    public static ChronicleEndUI Instance { get; private set; }
    
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private UIExtension openChronicleUI;
    [SerializeField] private UpdateScores updateScores;
    [SerializeField] private TextMeshProUGUI updateChronicle;
    [SerializeField] private TypewriterEffect typewriterEffect;
    
    [SerializeField] private List<ParticleSystem> fireWorks;
    [SerializeField] private GameObject fireWorkParent;
    
    private void Awake()
    {
        transform.parent = null;
        // Ensure the singleton is set
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void OpenEndChronicleUI(string waveStatus)
    {
        StartCoroutine(OpenEndChronicleChor(waveStatus));
    }

    IEnumerator OpenEndChronicleChor(string waveStatus)
    {
        PlayerReferenceManager.Instance.SetPlayerInMenus(true, "EndChronicle");
        typewriterEffect.StartTyping(waveStatus);
        
        yield return new WaitForSeconds(4f);

        typewriterEffect.DisplayText.text = "";

        //updateChronicle.text = "Chronicle\n" + GameDataManager.Instance.CurrentChronicle + "/4";
        openChronicleUI.FadeIn();
        updateScores.UpdateScoreUI(GameDataManager.Instance.GetCurrentChronicleData());
        yield return new WaitForSeconds(1);
        typewriterEffect.DisplayText.text = "";
    }
    
    public void CloseEndChronicleUI()
    {
        PlayerReferenceManager.Instance.SetPlayerInMenus(false);
        openChronicleUI.FadeOut();
        
    }
    
    
}
