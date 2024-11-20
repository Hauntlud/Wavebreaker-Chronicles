using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Settings")]
    public AudioSource musicSource; // The audio source for music playback
    public AudioCollection musicCollection; // Reference to the music audio collection

    private List<int> tempClipIndices = new List<int>(); // Temporary list to store unused clip indices
    private int currentClipIndex = -1; // Track the current playing clip
    private bool isPlaying = false;

    private void Awake()
    {
        // Ensure singleton instance
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

        // Initialize the tempClipIndices list with all available clips
        InitializeTempClipList();
    }


    
    public void PlayMusic(int clipIndex)
    {
        if (musicCollection == null || clipIndex < 0 || clipIndex >= musicCollection.audioClips.Count)
        {
            Debug.LogWarning("Invalid clip index or missing music collection.");
            return;
        }

        // Check if the selected track is already playing
        if (isPlaying && currentClipIndex == clipIndex)
        {
            return; // Already playing the same track
        }

        // Stop the current track if something is playing
        StopMusic();

        // Set the new music track
        currentClipIndex = clipIndex;
        AudioClip musicClip = musicCollection.GetClipByIndex(clipIndex);
        musicSource.clip = musicClip;

        // Play the music
        musicSource.loop = true;
        musicSource.Play();
        isPlaying = true;
    }
    
    public void PlayRandomMusic()
    {
        if (musicCollection == null || musicCollection.audioClips.Count == 0)
        {
            Debug.LogWarning("No audio clips available in the music collection.");
            return;
        }

        // Ensure we don't play the same song until all have been played
        if (tempClipIndices.Count == 0)
        {
            InitializeTempClipList(); // Refill the list if all clips have been played
        }

        // Randomly select a track from the available unused clips
        int randomIndex = Random.Range(0, tempClipIndices.Count);
        int clipIndex = tempClipIndices[randomIndex];

        // Remove the chosen clip from the temporary list
        tempClipIndices.RemoveAt(randomIndex);

        // Play the selected track
        PlayMusic(clipIndex);
    }
    
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        isPlaying = false;
        currentClipIndex = -1;
    }
    
    public bool IsMusicPlaying()
    {
        return isPlaying;
    }
    
    private void InitializeTempClipList()
    {
        tempClipIndices.Clear();
        for (int i = 0; i < musicCollection.audioClips.Count; i++)
        {
            tempClipIndices.Add(i);
        }
    }
}
