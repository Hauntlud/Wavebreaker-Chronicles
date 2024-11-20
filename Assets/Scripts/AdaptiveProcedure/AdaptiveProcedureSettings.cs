using UnityEngine;

[CreateAssetMenu(fileName = "New Adaptive Procedure Settings", menuName = "AdaptiveProcedure/Settings")]
public class AdaptiveProcedureSettings : ScriptableObject
{
    [Header("Step Size Settings")]
    public float maxStepSize = 0.2f;  // Initial step size
    public float minStepSize = 0.02f; // Minimum step size

    [Header("Response Thresholds")]
    public int correctResponsesNeeded = 2;  // Example: 2-up
    public int incorrectResponsesNeeded = 1; // Example: 1-down

    [Header("Reversal Settings")]
    public int setupReversals = 1;  // Number of reversals before reducing step size

    [Header("Step Reduction Settings")]
    public int stepReductionSteps = 10;  // Number of reversals required to reduce the step size to the minimum

    [Header("Initial Difficulty")]
    [Range(0f, 1f)] public float initialDifficulty = 0;  // Initial difficulty level between 0 and 1

    [Header("Difficulty Range")]
    public float minDifficulty = 0f; // Minimum difficulty
    public float maxDifficulty = 1f; // Maximum difficulty
}