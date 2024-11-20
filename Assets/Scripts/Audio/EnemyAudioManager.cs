using UnityEngine;

public class EnemyAudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource enemyAudioSource; // Each enemy has its own AudioSource

    [Header("Audio Collections")]
    [SerializeField] private AudioCollection enemySpawnAudio;
    [SerializeField] private AudioCollection enemyAttackAudio;
    [SerializeField] private AudioCollection enemyDamagedAudio;
    [SerializeField] private AudioCollection enemyDeathAudio;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float volume = 1f;

    private void Awake()
    {
        if (enemyAudioSource == null)
        {
            Debug.LogError("AudioSource component missing on the enemy!");
        }
    }

    // Play the sound when the enemy spawns
    public void PlayEnemySpawnAudio()
    {
        PlayRandomClipFromCollection(enemySpawnAudio);
    }

    // Play the sound when the enemy attacks
    public void PlayEnemyAttackAudio()
    {
        PlayRandomClipFromCollection(enemyAttackAudio);
    }

    // Play the sound when the enemy is damaged
    public void PlayEnemyDamagedAudio()
    {
        PlayRandomClipFromCollection(enemyDamagedAudio);
    }

    // Play the sound when the enemy dies
    public void PlayEnemyDeathAudio()
    {
        PlayRandomClipFromCollection(enemyDeathAudio);
    }

    // Play a random clip from the provided AudioCollection
    private void PlayRandomClipFromCollection(AudioCollection audioCollection)
    {
        if (audioCollection == null || audioCollection.audioClips.Count == 0)
        {
            Debug.LogWarning("No audio clips available in the provided audio collection.");
            return;
        }

        // Pick a random clip from the collection
        AudioClip clipToPlay = audioCollection.GetRandomClip();
        if (clipToPlay == null)
        {
            Debug.LogWarning("Selected audio clip is null.");
            return;
        }

        // Play the clip on the enemy's audio source
        enemyAudioSource.clip = clipToPlay;
        enemyAudioSource.volume = audioCollection.volume;
        enemyAudioSource.Play();
    }
}
