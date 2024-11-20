using UnityEngine;
using System.Collections.Generic;
using Firebase.Analytics;

public class FirebaseEventManager : MonoBehaviour
{
    public static FirebaseEventManager Instance { get; private set; }
    
    private void Awake()
    {
        transform.parent = null;
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

    private void Start()
    {
        FirebaseManager.Instance.OnFirebaseInitialized += ProcessQueuedEvents;
    }

    // Queue for pending events if Firebase is not initialized
    private List<System.Action> eventQueue = new List<System.Action>();

    public void LogMainMenuEvent() => QueueOrLog(() => FirebaseManager.Instance.LogCustomEvent("main_menu"));
    public void LogGameOpenEvent() => QueueOrLog(() => FirebaseManager.Instance.LogCustomEvent("game_open"));
    public void LogGameQuitEvent() => QueueOrLog(() => FirebaseManager.Instance.LogCustomEvent("game_quit"));
    public void LogGameStartEvent() => QueueOrLog(() => FirebaseManager.Instance.LogCustomEvent("game_start"));
    public void LogSkillsScreenEvent() => QueueOrLog(() => FirebaseManager.Instance.LogCustomEvent("skills_screen"));
    public void LogSettingsMenuEvent() => QueueOrLog(() => FirebaseManager.Instance.LogCustomEvent("settings_menu"));
    public void LogChronicleEvent(int chronicleIndex, float playtime) 
        => QueueOrLog(() => FirebaseManager.Instance.LogCustomEvent("chronicle_event", 
            new Parameter("chronicle_index", chronicleIndex),
            new Parameter("playtime", playtime)));

    private void QueueOrLog(System.Action logAction)
    {
        if (FirebaseManager.Instance.IsInitialized)
        {
            logAction.Invoke();
        }
        else
        {
            eventQueue.Add(logAction);
        }
    }

    private void ProcessQueuedEvents()
    {
        foreach (var logAction in eventQueue)
        {
            logAction.Invoke();
        }
        eventQueue.Clear();
    }
}