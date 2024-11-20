using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AdaptiveProcedure : MonoBehaviour
{
    [SerializeField] private AdaptiveProcedureSettings settings;  // Reference to ScriptableObject

    [SerializeField] private float currentDifficulty;
    [SerializeField] private float currentStepSize;
    [SerializeField] private int correctResponseCounter = 0;
    [SerializeField] private int incorrectResponseCounter = 0;
    [SerializeField] private int reversalCounter = 0;
    [SerializeField] private bool lastResponseWasCorrect = false;

    // List to track the difficulty on a reversal after reaching minimum step size
    [SerializeField] private List<float> recordedDifficulties = new List<float>();
    
    public void SetSettings(AdaptiveProcedureSettings settingsIn)
    {
        settings = settingsIn;
    }

    // Initialize the adaptive procedure with initial values from the settings
    public void InitializeProcedure()
    {
        // Reset difficulty and step size to their initial values
        currentDifficulty = settings.initialDifficulty;
        currentStepSize = settings.maxStepSize;

        // Reset counters and flags
        correctResponseCounter = 0;
        incorrectResponseCounter = 0;
        reversalCounter = 0;
        lastResponseWasCorrect = false;

        // Clear recorded difficulties to start fresh
        recordedDifficulties.Clear();
    }

    public void SetDifficulty(float difficulty)
    {
        
        currentDifficulty = Mathf.Clamp(difficulty,settings.minDifficulty,settings.maxDifficulty);
        //print("currentDifficulty " + currentDifficulty);
        //print("Difficulty IN " + difficulty);
    }

    // Method to register a response (true = correct, false = incorrect)
    public void RegisterResponse(bool isCorrect)
    {
        if (isCorrect)
        {
            correctResponseCounter++;
            incorrectResponseCounter = 0;  // Reset incorrect responses

            if (correctResponseCounter >= settings.correctResponsesNeeded)
            {
                AdjustDifficulty(true);
                correctResponseCounter = 0;  // Reset correct counter after adjusting difficulty
            }
        }
        else
        {
            incorrectResponseCounter++;
            correctResponseCounter = 0;  // Reset correct responses

            if (incorrectResponseCounter >= settings.incorrectResponsesNeeded)
            {
                AdjustDifficulty(false);
                incorrectResponseCounter = 0;  // Reset incorrect counter after adjusting difficulty
            }
        }

        // Check for reversal
        CheckReversal(isCorrect);
    }

    // Adjust difficulty based on the response (true = increase, false = decrease)
    private void AdjustDifficulty(bool increase)
    {
        if (increase)
        {
            currentDifficulty = Mathf.Clamp(currentDifficulty + currentStepSize, settings.minDifficulty, settings.maxDifficulty);
        }
        else
        {
            currentDifficulty = Mathf.Clamp(currentDifficulty - currentStepSize, settings.minDifficulty, settings.maxDifficulty);
        }

        //Debug.Log($"Current Difficulty: {currentDifficulty}");
    }

    // Check for reversals and reduce step size after enough reversals
    private void CheckReversal(bool isCorrect)
    {
        if (lastResponseWasCorrect != isCorrect)
        {
            reversalCounter++;

            if (reversalCounter >= settings.setupReversals)
            {
                // Reduce step size by a fraction of the max-min step size, over the configured number of steps from settings
                float stepSizeReduction = (settings.maxStepSize - settings.minStepSize) / settings.stepReductionSteps;

                currentStepSize = Mathf.Max(currentStepSize - stepSizeReduction, settings.minStepSize);
                reversalCounter = 0;  // Reset reversal counter

                //Debug.Log($"Step size reduced to: {currentStepSize}");

                // Check if we've reached the minimum step size and start recording
                if (currentStepSize == settings.minStepSize)
                {
                    RecordDifficulty();
                }
            }
        }

        lastResponseWasCorrect = isCorrect;
    }

    // Record the difficulty only on reversal when step size is at the minimum
    private void RecordDifficulty()
    {
        recordedDifficulties.Add(currentDifficulty);
        //Debug.Log($"Recording difficulty on reversal: {currentDifficulty}");
    }

    // Method to get the current difficulty
    public float GetCurrentDifficulty()
    {
        // If there are recorded difficulties, return the median of those, otherwise return the current difficulty
        if (recordedDifficulties.Count > 0)
        {
            return GetMedian(recordedDifficulties);
        }
        return currentDifficulty;
    }
    
    public float GetNormOfCurrentDifficulty()
    {
        return Mathf.InverseLerp(settings.minDifficulty,settings.maxDifficulty,GetCurrentDifficulty());
    }

    // Helper method to calculate the median of a list of floats
    private float GetMedian(List<float> values)
    {
        var sortedValues = values.OrderBy(v => v).ToList();
        int count = sortedValues.Count;

        if (count % 2 == 0)
        {
            // Even number of elements, return the average of the middle two
            return (sortedValues[count / 2 - 1] + sortedValues[count / 2]) / 2f;
        }
        else
        {
            // Odd number of elements, return the middle one
            return sortedValues[count / 2];
        }
    }

    // Method to retrieve the recorded difficulties
    public List<float> GetRecordedDifficulties()
    {
        return recordedDifficulties;
    }
}
