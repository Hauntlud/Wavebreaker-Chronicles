using System.Collections;
using UnityEngine;

public class ObjectScalerUpDown : MonoBehaviour
{
    [SerializeField] private float scaleUpMultiplier = 1.2f;  // How much to scale up
    [SerializeField] private float scaleDuration = 0.1f;      // How long to scale up and down
    [SerializeField] private Vector3 defaultScale;            // The original scale of the character

    private Coroutine scaleCoroutine;

    private void Start()
    {
        // Save the default scale of the character on start
        defaultScale = transform.localScale;
    }

    // This method can be called when shooting or attacking
    public void TriggerScaleEffect()
    {
        // If a scaling effect is already happening, stop it before starting a new one
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        // Start the scale effect coroutine
        scaleCoroutine = StartCoroutine(ScaleUpAndDown());
    }

    private IEnumerator ScaleUpAndDown()
    {
        // Scale up the character
        Vector3 targetScale = defaultScale * scaleUpMultiplier;
        float elapsed = 0f;

        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, elapsed / scaleDuration);
            yield return null;
        }

        transform.localScale = targetScale;

        // Scale back down to the original size
        elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(transform.localScale, defaultScale, elapsed / scaleDuration);
            yield return null;
        }

        // Ensure it's set exactly back to default scale at the end
        transform.localScale = defaultScale;

        // Reset the coroutine
        scaleCoroutine = null;
    }
}