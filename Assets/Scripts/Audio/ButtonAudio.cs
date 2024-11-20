using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour
{
    private Button button;  // Reference to the button component

    private void Awake()
    {
        // Get the button component on the object this script is attached to
        button = GetComponent<Button>();
        
        // Add the listener for playing the button press audio
        button.onClick.AddListener(PlayButtonPressAudio);
    }

    private void PlayButtonPressAudio()
    {
        // Call the PlayButtonPress method from UIAudioManager to play the button press sound
        if (UIAudioManager.Instance != null)
        {
            UIAudioManager.Instance.PlayButtonPress();
        }
        else
        {
            Debug.LogWarning("UIAudioManager is not set or not found.");
        }
    }

    private void OnDestroy()
    {
        // Remove the listener when the object is destroyed to avoid potential memory leaks
        button.onClick.RemoveListener(PlayButtonPressAudio);
    }
}