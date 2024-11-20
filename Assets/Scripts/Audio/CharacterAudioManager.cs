using UnityEngine;
using UnityEngine.Serialization;

public class CharacterAudioManager : MonoBehaviour
{

    [SerializeField] private AudioSource eventAudioSource;
    [SerializeField] private AudioCollection damagedAudio;
    [SerializeField] private AudioCollection meleeAttackAudio;
    [SerializeField] private AudioCollection rangeAttackAudio;
    [SerializeField] private AudioCollection healingAudio;
    [SerializeField] private AudioCollection levelUpAudio;
    [SerializeField] private AudioCollection spawnAudio;
    [SerializeField] private AudioCollection deathAudio;
    [SerializeField] private AudioCollection speedAudio;
    [SerializeField] private AudioCollection xpAudio;
    
    private void Start()
    {
        PlaySpawnAudio();
    }

    // Player event methods
    public void PlayDamagedAudio() => PlayRandomClipFromCollection(damagedAudio);
    public void PlayPickUpSpeedSound() => PlayOnceFromCollection(speedAudio);
    public void PlayPickUpXPSound() => PlayRandomClipFromCollection(xpAudio);
    public void PlayLevelUpAudio() => PlayOnceFromCollection(levelUpAudio);
    public void PlayMeleeAttackAudio() => PlayRandomClipFromCollection(meleeAttackAudio);
    public void PlayRangeAttackAudio() => PlayRandomClipFromCollection(rangeAttackAudio);
    public void PlayHealingAudio() => PlayRandomClipFromCollection(healingAudio);
    public void PlaySpawnAudio() => PlayOnceFromCollection(spawnAudio);
    public void PlayDeathAudio() => PlayRandomClipFromCollection(deathAudio);

    // Generic method for playing a random clip from a provided audio collection
    public void PlayRandomClipFromCollection(AudioCollection audioCollection)
    {
        if (audioCollection == null || audioCollection.audioClips.Count == 0)
        {
            Debug.LogWarning("No audio clips available in the provided audio collection.");
            return;
        }

        AudioClip randomClip = audioCollection.GetRandomClip();
        if (audioCollection.randomizePitch)
        {
            eventAudioSource.pitch = Random.Range(audioCollection.minPitch, audioCollection.maxPitch);
        }

        eventAudioSource.clip = randomClip;
        eventAudioSource.Play();
    }
    
    public void PlayOnceFromCollection(AudioCollection audioCollection)
    {
        if (audioCollection == null || audioCollection.audioClips.Count == 0)
        {
            Debug.LogWarning("No audio clips available in the provided audio collection.");
            return;
        }

        AudioClip randomClip = audioCollection.GetRandomClip();
        if (audioCollection.randomizePitch)
        {
            eventAudioSource.pitch = Random.Range(audioCollection.minPitch, audioCollection.maxPitch);
        }

        eventAudioSource.PlayOneShot(randomClip);
    }

    // Stop any currently playing player event audio
    public void StopCurrentAudio()
    {
        if (eventAudioSource.isPlaying)
        {
            eventAudioSource.Stop();
        }
    }
}
