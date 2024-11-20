using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Audio Collection", menuName = "Audio/Audio Collection")]
public class AudioCollection : ScriptableObject
{
    [Header("Audio Clips")]
    public List<AudioClip> audioClips; // List of audio clips

    [Header("Audio Settings")]
    [Range(0f, 1f)] public float volume = 1f; // Volume for all clips
    [Range(0.5f, 2f)] public float pitch = 1f; // Base pitch
    public bool randomizePitch = false; // If true, randomize pitch within a range
    [Range(0.5f, 2f)] public float minPitch = 0.9f; // Minimum pitch value for randomization
    [Range(0.5f, 2f)] public float maxPitch = 1.1f; // Maximum pitch value for randomization
    public bool isLoop = false; // Maximum pitch value for randomization

    /// <summary>
    /// Picks a random clip from the list.
    /// </summary>
    public AudioClip GetRandomClip()
    {
        if (audioClips.Count == 0) return null;
        return audioClips[Random.Range(0, audioClips.Count)];
    }

    /// <summary>
    /// Picks a specific clip by index.
    /// </summary>
    public AudioClip GetClipByIndex(int index)
    {
        if (index < 0 || index >= audioClips.Count) return null;
        return audioClips[index];
    }
}