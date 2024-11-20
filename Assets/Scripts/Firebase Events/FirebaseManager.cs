using UnityEngine;
using Firebase;
using Firebase.Analytics;
using System.Collections.Generic;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }
    public bool IsInitialized { get; private set; } = false;
    public event System.Action OnFirebaseInitialized;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFirebase();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                IsInitialized = true;
                OnFirebaseInitialized?.Invoke();
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies.");
            }
        });
    }

    public void LogCustomEvent(string eventName, params Parameter[] parameters)
    {
        if (IsInitialized)
        {
            FirebaseAnalytics.LogEvent(eventName, parameters);
            print("Event Sent " + eventName);
        }
        else
        {
            Debug.LogWarning($"Event '{eventName}' queued as Firebase is not yet initialized.");
        }
    }
}