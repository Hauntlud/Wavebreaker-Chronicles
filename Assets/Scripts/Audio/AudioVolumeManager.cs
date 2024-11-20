using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioVolumeManager : MonoBehaviour
{
    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup uiMixerGroup;

    [Header("Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider uiVolumeSlider;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button acceptButton;

    [Header("Audio Clips")]
    [SerializeField] private AudioCollection sliderChangeSound;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sliderAudioSource;  // AudioSource for slider sounds

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string UIVolumeKey = "UIVolume";

    // Default volumes (normalized)
    private const float defaultMasterVolume = 1f;
    private const float defaultMusicVolume = 0.7f;
    private const float defaultSFXVolume = 0.9f;
    private const float defaultUIVolume = 1f;

    private void Start()
    {
        // Initialize sliders with saved values or defaults
        InitializeVolumeSlider(masterVolumeSlider, MasterVolumeKey, "MasterVolume", defaultMasterVolume);
        InitializeVolumeSlider(musicVolumeSlider, MusicVolumeKey, "MusicVolume", defaultMusicVolume);
        InitializeVolumeSlider(sfxVolumeSlider, SFXVolumeKey, "SFXVolume", defaultSFXVolume);
        InitializeVolumeSlider(uiVolumeSlider, UIVolumeKey, "UIVolume", defaultUIVolume);

        // Add listeners for each slider
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        uiVolumeSlider.onValueChanged.AddListener(SetUIVolume);

        // Button listeners
        resetButton.onClick.AddListener(ResetVolume);
        acceptButton.onClick.AddListener(SaveVolumeSettings);
    }

    private void InitializeVolumeSlider(Slider slider, string playerPrefKey, string mixerParam, float defaultValue)
    {
        // Load saved volume from PlayerPrefs or use the default value
        float savedVolume = PlayerPrefs.HasKey(playerPrefKey) ? PlayerPrefs.GetFloat(playerPrefKey) : defaultValue;
        slider.value = savedVolume;
        SetVolume(mixerParam, savedVolume);
    }

    public void ResetVolume()
    {
        // Reset sliders to default values
        masterVolumeSlider.value = defaultMasterVolume;
        musicVolumeSlider.value = defaultMusicVolume;
        sfxVolumeSlider.value = defaultSFXVolume;
        uiVolumeSlider.value = defaultUIVolume;

        // Apply and save
        SetMasterVolume(defaultMasterVolume);
        SetMusicVolume(defaultMusicVolume);
        SetSFXVolume(defaultSFXVolume);
        SetUIVolume(defaultUIVolume);
    }

    public void SetMasterVolume(float value)
    {
        SetVolume("MasterVolume", value);
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        PlaySliderChangeSound(masterMixerGroup);
    }

    public void SetMusicVolume(float value)
    {
        SetVolume("MusicVolume", value);
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlaySliderChangeSound(musicMixerGroup);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume("SFXVolume", value);
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
        PlaySliderChangeSound(sfxMixerGroup);
    }

    public void SetUIVolume(float value)
    {
        SetVolume("UIVolume", value);
        PlayerPrefs.SetFloat(UIVolumeKey, value);
        PlaySliderChangeSound(uiMixerGroup);
    }

    private void SetVolume(string mixerParameter, float sliderValue)
    {
        // Convert slider value (0 to 1) to a logarithmic scale (-80 dB to 0 dB)
        float volumeInDb = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 60f;
        masterMixer.SetFloat(mixerParameter, volumeInDb);
    }

    private void PlaySliderChangeSound(AudioMixerGroup mixerGroup)
    {
        if (sliderChangeSound != null && sliderAudioSource != null)
        {
            sliderAudioSource.outputAudioMixerGroup = mixerGroup;

            if (!sliderAudioSource.isPlaying)
            {
                sliderAudioSource.clip = sliderChangeSound.audioClips[Random.Range(0,sliderChangeSound.audioClips.Count)];
                sliderAudioSource.Play();
            }


        }
    }

    private void SaveVolumeSettings()
    {
        // Save all volume settings
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        // Ensure volume settings are saved before quitting
        PlayerPrefs.Save();
    }
}
