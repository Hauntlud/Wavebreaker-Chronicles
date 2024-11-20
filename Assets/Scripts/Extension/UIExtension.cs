using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;
using UnityEngine.Serialization;

public class UIExtension : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private GameObject toggleGameObject;
    [SerializeField] private TextMeshProUGUI textPro;
    [SerializeField] private RectTransform uiElement; 
    [SerializeField] private CanvasGroup canvasGroup; // Reference to the CanvasGroup to fade
    [SerializeField] private Button button; // Reference to the CanvasGroup to fade
    [SerializeField] private List<Sprite> ghostImage; // Reference to the CanvasGroup to fade
    
    
    [SerializeField] private float shakeDuration = 1f; 
    [SerializeField] private float shakeMagnitude = 10f; 
    [SerializeField] private float lerpDuration = 0.5f;
    
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private float overshootAmount = 1.2f; // How much bigger it gets before shrinking to target
    [SerializeField] private float targetScale = 1f;  // The final scale value
    [SerializeField] private float shrinkScale = 0f;  // The scale to shrink down to

    public Image Image => image;
    public CanvasGroup CanvasGroup => canvasGroup;
    public TextMeshProUGUI TextPro => textPro;
    public Button Button => button;
    
    private Vector3 startingLocation;
    private Coroutine lerpFillCoroutine;
    private Coroutine lerpRotationCoroutine;
    private Coroutine scaleCoroutine;
    private Coroutine shakeCoroutine;
    private Coroutine lerpTextCoroutine;
    private Coroutine fadeCoroutine; // To handle stopping the fade coroutine if needed
    
    // Called when the script is reset
    private void Reset()
    {
        image = GetComponent<Image>();
        uiElement = GetComponent<RectTransform>();
        textPro = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        toggleGameObject = gameObject;
        if (image == null)
        {
            Debug.LogWarning("No Image component found. Ensure this GameObject has an Image attached.");
        }
    }

    // Called on script start
    private void Start()
    {

        if (uiElement != null)
        {
            startingLocation = uiElement.localPosition;
        }
        
    }

// Method to fade the CanvasGroup in over time
    public void FadeIn()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        canvasGroup.alpha = 0;
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1f, lerpDuration));
    }

// Method to fade the CanvasGroup out over time
    public void FadeOut()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        canvasGroup.alpha = 1;
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0f, lerpDuration));
    }

// Coroutine to handle fading the CanvasGroup
    private IEnumerator FadeCanvasGroup(CanvasGroup group, float start, float end, float duration)
    {
        float elapsedTime = 0f;

        if (Mathf.Approximately(end, 1))
        {
            ToggleOn(true);
        }
        

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }

        group.alpha = end;

        // Disable interactability if faded out
        if (end == 0f)
        {
            ToggleOn(false);
        }
    }


    // Public method to start the lerp to a target value
    public void LerpToNumber(float targetValue)
    {
        // Stop any ongoing lerp to prevent multiple coroutines from interfering
        if (lerpTextCoroutine != null)
        {
            StopCoroutine(lerpTextCoroutine);
        }

        if (!textPro.isActiveAndEnabled) return;

        // Ensure textPro has a valid initial value
        if (string.IsNullOrEmpty(textPro.text))
        {
            textPro.text = "0";  // Set a default starting value if it's empty
        }

        // Log the target value to ensure it's correct
        //Debug.Log(transform.name + " Target value for Lerp: " + targetValue);

        // Start a new lerp
        lerpTextCoroutine = StartCoroutine(LerpNumberCoroutine(targetValue));
    }

    private IEnumerator LerpNumberCoroutine(float targetValue)
    {
        // Try to parse the current text to get the starting number. Default to 0 if parsing fails.
        float startValue = 0f;
        if (float.TryParse(textPro.text, out float currentNumber))
        {
            startValue = currentNumber;
        }

        float elapsedTime = 0f;

        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / lerpDuration);

            // Lerp between the start and target values
            float newValue = Mathf.Lerp(startValue, targetValue, progress);

            // Update the text with the new value (rounded to avoid long decimal numbers)
            textPro.text = Mathf.RoundToInt(newValue).ToString();

            yield return null;
        }

        // Ensure the final value is set precisely at the end
        textPro.text = Mathf.RoundToInt(targetValue).ToString();
    }
    

    public void ToggleOn(bool toggle)
    {
        toggleGameObject.SetActive(toggle);
    }
    
        [Button("Test Scale Up")]
    public void ScaleUp()
    {
        // If a scale coroutine is already running, stop it
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        scaleCoroutine = StartCoroutine(ScaleUpCoroutine());
    }

    [Button("Test Scale Down")]
    public void ScaleDown()
    {
        // If a scale coroutine is already running, stop it
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        scaleCoroutine = StartCoroutine(ScaleDownCoroutine());
    }

    [Button("Test Scale Up and Down")]
    public void ScaleUpAndDown()
    {
        // If a scale coroutine is already running, stop it
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        scaleCoroutine = StartCoroutine(ScaleUpAndDownCoroutine());
    }

    // Coroutine to scale the UI element up with an overshoot effect
    private IEnumerator ScaleUpCoroutine()
    {
        float timeElapsed = 0f;
        Vector3 initialScale = uiElement.localScale;
        Vector3 overshootScale = new Vector3(overshootAmount, overshootAmount, overshootAmount);
        Vector3 finalScale = new Vector3(targetScale, targetScale, targetScale);

        // Scale up past the target with an overshoot
        while (timeElapsed < scaleDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / scaleDuration;
            uiElement.localScale = Vector3.Lerp(initialScale, overshootScale, progress);
            yield return null;
        }

        // Now shrink back to the target scale
        timeElapsed = 0f;
        while (timeElapsed < scaleDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / scaleDuration;
            uiElement.localScale = Vector3.Lerp(overshootScale, finalScale, progress);
            yield return null;
        }

        uiElement.localScale = finalScale;
        scaleCoroutine = null;  // Reset the coroutine reference
    }

    // Coroutine to scale the UI element down to zero
    private IEnumerator ScaleDownCoroutine()
    {
        float timeElapsed = 0f;
        Vector3 initialScale = uiElement.localScale;
        Vector3 finalScale = new Vector3(shrinkScale, shrinkScale, shrinkScale);

        while (timeElapsed < scaleDuration)
        {
            timeElapsed += Time.deltaTime;
            float progress = timeElapsed / scaleDuration;
            uiElement.localScale = Vector3.Lerp(initialScale, finalScale, progress);
            yield return null;
        }

        uiElement.localScale = finalScale;
        scaleCoroutine = null;  // Reset the coroutine reference
    }

    // Coroutine to scale up with overshoot and then scale back down to zero
    private IEnumerator ScaleUpAndDownCoroutine()
    {
        // Scale up with overshoot first
        yield return ScaleUpCoroutine();

        // Scale down after a slight delay
        yield return new WaitForSeconds(0.1f);

        // Start shrinking down to zero
        yield return ScaleDownCoroutine();

        scaleCoroutine = null;  // Reset the coroutine reference
    }
    
    [Button("Test Shake")]
    public void ShakeUIElement()
    {
        // If the shake is already running, stop it before starting a new one
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }

        // Start the shake coroutine
        shakeCoroutine = StartCoroutine(Shake(shakeDuration, shakeMagnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = startingLocation;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            uiElement.localPosition = new Vector3(originalPosition.x + offsetX, originalPosition.y + offsetY, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset position after shake
        uiElement.localPosition = originalPosition;
        shakeCoroutine = null;  // Reset the coroutine reference
    }

    public void SetLerpDuriation(float duration)
    {
        lerpDuration = duration;
    }

    // Set an image reference and interact with it
    public void SetImageReference(Image newImage)
    {
        if (newImage == null)
        {
            Debug.LogWarning("Provided Image reference is null.");
            return;
        }

        image = newImage;
    }

    // Lerp the fill amount to a new value over time
    public void LerpFillAmount(float targetFill)
    {
        if (image == null)
        {
            Debug.LogWarning("No Image reference set for LerpFillAmount.");
            return;
        }

        if (lerpFillCoroutine != null)
        {
            StopCoroutine(lerpFillCoroutine);
        }

        lerpFillCoroutine = StartCoroutine(LerpFillCoroutine(targetFill));
    }

    // Efficient coroutine to handle fill amount lerping
    private IEnumerator LerpFillCoroutine(float targetFill)
    {
        float startFill = image.fillAmount;
        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {
            timeElapsed += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(startFill, targetFill, timeElapsed / lerpDuration);
            yield return null; // Efficient frame update
        }

        image.fillAmount = targetFill; // Ensure it sets to the exact value at the end
    }

    // Set rotation on the Z axis
    public void SetRotationZ(float angle)
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Lerp rotation on the Z axis over time
    public void LerpRotationZ(float targetAngle)
    {
        if (lerpRotationCoroutine != null)
        {
            StopCoroutine(lerpRotationCoroutine);
        }

        lerpRotationCoroutine = StartCoroutine(LerpRotationCoroutine(targetAngle));
    }

    // Efficient coroutine to handle rotation lerping on Z axis
    private IEnumerator LerpRotationCoroutine(float targetAngle)
    {
        float startAngle = transform.rotation.eulerAngles.z;
        float timeElapsed = 0;

        while (timeElapsed < lerpDuration)
        {
            timeElapsed += Time.deltaTime;
            float newAngle = Mathf.Lerp(startAngle, targetAngle, timeElapsed / lerpDuration);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, targetAngle); // Ensure exact angle is set at the end
    }

    // Optional: A method to reset rotation on the Z axis
    public void ResetRotationZ()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    // Optional: A method to reset fill amount back to zero
    public void ResetFillAmount()
    {
        if (image == null)
        {
            Debug.LogWarning("No Image reference set for ResetFillAmount.");
            return;
        }

        image.fillAmount = 0;
    }
}
