using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance { get; private set; }

    [SerializeField] private AudioSource eventAudioSource;
    [SerializeField] private AudioCollection goodEventAudio;
    [SerializeField] private AudioCollection badEventAudio;
    [SerializeField] private AudioCollection buttonPress;
    [SerializeField] private AudioCollection celebrationAudio;
    [SerializeField] private AudioCollection startupAudio;

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
        PlayStartupEvent();
    }

    public void PlayGoodEvent() => PlayOneShotFromCollection(goodEventAudio);
    public void PlayBadEvent() => PlayRandomClipFromCollection(badEventAudio);
    public void PlayButtonPress() => PlayRandomClipFromCollection(buttonPress);
    public void PlayCelebrationEvent() => PlayRandomClipFromCollection(celebrationAudio);
    public void PlayStartupEvent() => PlayRandomClipFromCollection(startupAudio);

    private void PlayRandomClipFromCollection(AudioCollection audioCollection)
    {
        if (audioCollection == null || audioCollection.audioClips.Count == 0)
        {
            Debug.LogWarning("No audio clips available in the provided audio collection.");
            return;
        }

        if (audioCollection.randomizePitch)
        {
            eventAudioSource.pitch = Random.Range(audioCollection.minPitch, audioCollection.maxPitch);
        }
        
        eventAudioSource.clip = audioCollection.GetRandomClip();
        eventAudioSource.Play();
    }
    
    private void PlayOneShotFromCollection(AudioCollection audioCollection)
    {
        if (audioCollection == null || audioCollection.audioClips.Count == 0)
        {
            Debug.LogWarning("No audio clips available in the provided audio collection.");
            return;
        }

        if (audioCollection.randomizePitch)
        {
            eventAudioSource.pitch = Random.Range(audioCollection.minPitch, audioCollection.maxPitch);
        }
        
        eventAudioSource.PlayOneShot(audioCollection.GetRandomClip());
    }

    public void StopCurrentAudio()
    {
        if (eventAudioSource.isPlaying)
        {
            eventAudioSource.Stop();
        }
    }
}