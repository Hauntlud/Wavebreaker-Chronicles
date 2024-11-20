using UnityEngine;
using UnityEngine.UI;

public class PlayerDirectionIndicator : MonoBehaviour
{
    public RectTransform uiCircle;  // Reference to the UI circle
    
    private void Update()
    {
        if (PlayerReferenceManager.Instance.PlayerMovementController)
        {
            // Get the movement direction from the PlayerController
            Vector3 direction = PlayerReferenceManager.Instance.PlayerMovementController.MovementDirection;

            if (direction.magnitude > 0)
            {
                // Rotate the UI circle based on movement direction
                RotateUICircle(new Vector2(direction.x, direction.z));
            }
        }
    }

    private void RotateUICircle(Vector2 direction)
    {
        // Normalize direction to avoid erratic rotation and calculate angle
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate the UI circle based on the calculated angle
        uiCircle.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));  // Adjust for UI rotation
    }
}