using UnityEngine;

public class LocalZAxisRotation : MonoBehaviour
{
    private bool isRotating = false;
    private float elapsedTime = 0f;
    private float rotationDuration;
    private Quaternion initialRotation;
    private Quaternion targetRotation;

    void Update()
    {
        // Perform the rotation over time if the object is rotating
        if (isRotating)
        {
            PerformRotation();
        }
    }

    public void StartRotation(float rotationAmount, float duration)
    {
        gameObject.SetActive(true);
        // Save the initial rotation and compute the target rotation
        initialRotation = transform.localRotation;
        targetRotation = initialRotation * Quaternion.Euler(0f, 0f, rotationAmount);
        
        rotationDuration = duration;
        elapsedTime = 0f;
        isRotating = true;

    }

    void PerformRotation()
    {
        // Increment the time
        elapsedTime += Time.deltaTime;

        // Calculate the interpolation factor
        float t = Mathf.Clamp01(elapsedTime / rotationDuration);

        // Smoothly interpolate from the initial to the target rotation
        transform.localRotation = Quaternion.Lerp(initialRotation, targetRotation, t);

        // Stop rotating once the duration is reached
        if (t >= 1f)
        {
            isRotating = false;
            transform.localRotation = initialRotation;
            gameObject.SetActive(false);
        }
    }
}