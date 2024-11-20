using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText; // Reference to the UI Text component where the word will be typed
    [SerializeField] private float typingSpeed = 0.1f; // Time delay between each letter
    private Coroutine typingCoroutine; // To keep track of the coroutine

    public TextMeshProUGUI DisplayText => displayText;
    
    // Function to start typing out a word letter by letter
    public void StartTyping(string word)
    {
        // Stop any currently running typing coroutine
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Start a new coroutine to type the text
        typingCoroutine = StartCoroutine(TypeText(word));
    }

    // Coroutine to handle the typing effect
    private IEnumerator TypeText(string word)
    {
        displayText.text = ""; // Clear the existing text first
        foreach (char letter in word)
        {
            displayText.text += letter; // Append each letter one by one
            yield return new WaitForSeconds(typingSpeed); // Wait for the specified time before typing the next letter
        }
        // When typing is done, reset the coroutine reference
        typingCoroutine = null;
    }
}