using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    
    public void PlayClip(AudioCollection audioCollection, int clipIndex)
    {
        if (audioSource == null || audioCollection == null) return;

        // Get the clip from the ScriptableObject
        AudioClip clipToPlay = audioCollection.GetClipByIndex(clipIndex);
        if (clipToPlay == null) return;

        // Configure the AudioSource
        audioSource.clip = clipToPlay;
        audioSource.volume = audioCollection.volume;
        audioSource.pitch = audioCollection.randomizePitch
            ? Random.Range(audioCollection.minPitch, audioCollection.maxPitch)
            : audioCollection.pitch;
        audioSource.loop = audioCollection.isLoop;

        // Play the audio clip
        audioSource.Play();
    }

    public void StopClip()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public bool IsPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }
}