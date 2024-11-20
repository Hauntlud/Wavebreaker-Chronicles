using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementController : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private Vector3 movementDirection;
    [SerializeField] private Vector3 rotationDirection;  // Store the rotation direction
    private Vector3 aimDirection;  // Direction to aim
    private Camera mainCamera;

    private Vector2 startTouchPosition;  // For movement input (mobile)
    private Vector2 secondTouchPosition;  // For aiming input (mobile)

    [SerializeField] private float moveSpeed = 5f;  // Set this or get from PlayerController
    [SerializeField] private TargetingSystem targetingSystem; // Set this or get from PlayerController
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float maxDistance;
    [SerializeField] private float rotationSpeed = 7;
    public Vector3 MovementDirection => movementDirection;  // Getter for movement direction
    public Vector3 RotationDirection => rotationDirection;  // Getter for rotation direction

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (PlayerReferenceManager.Instance.playerController.Isdead || PlayerReferenceManager.Instance.PlayerInMenus)
        {
            movementDirection = new Vector3(0, 0, 0);
            return;
        }
        if (PlayerReferenceManager.Instance.IsMobile)
        {
            HandleMobileInput();
        }
        else
        {
            HandlePCInput();
        }
    }

    private void FixedUpdate()
    {
        if (PlayerReferenceManager.Instance.playerController.Isdead || PlayerReferenceManager.Instance.PlayerInMenus) return;
        if (PlayerReferenceManager.Instance.IsMobile)
        {
            Transform closestEnemy = targetingSystem.GetClosestEnemy(transform.position);
            if (closestEnemy != null)
            {
                // If there is an enemy, rotate towards it
                RotatePlayer((closestEnemy.position - transform.position).normalized);
            }

        }

        // Move the player based on movement direction
        MovePlayer();
    }

    private void HandlePCInput()
    {
        // WASD movement input
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        movementDirection = new Vector3(moveHorizontal, 0, moveVertical).normalized;

        // Mouse aiming direction
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position.y);  // Ground plane at player's Y position
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 worldPoint = ray.GetPoint(rayDistance);
            aimDirection = (worldPoint - transform.position).normalized;  // Update aim direction
            MoveToClosestPosition(worldPoint);
            RotatePlayer(aimDirection);
        }

        //RotatePlayer(aimDirection);  // Rotate player based on mouse direction
    }
    
    

    private void HandleMobileInput()
    {
        
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (PlayerReferenceManager.Instance.IsPointerOverUIElementMobile(touch)) return;
            
            // Determine if the touch is on the left or right side of the screen
            if (touch.position.x < Screen.width / 2)
            {
                // Left side of the screen - Movement
                HandleMovementInput(touch);
            }
            else
            {
                // Right side of the screen - Rotation
                HandleRotationInput(touch);
            }
        }

        // If no touches, reset movement direction to stop player movement
        if (Input.touchCount == 0)
        {
            movementDirection = Vector3.zero;
        }
    }

    private void HandleMovementInput(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            startTouchPosition = touch.position;
        }
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            Vector2 movementDragDirection = (touch.position - startTouchPosition).normalized;
            Vector3 worldMovementDirection = new Vector3(movementDragDirection.x, 0, movementDragDirection.y);

            movementDirection = worldMovementDirection;
        }
    }

    private void HandleRotationInput(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            secondTouchPosition = touch.position;
        }
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            Vector2 rotationDragDirection = (touch.position - secondTouchPosition).normalized;
            Vector3 worldRotationDirection = new Vector3(rotationDragDirection.x, 0, rotationDragDirection.y);

            // Set the target position at a fixed distance from the player in the drag direction
            float targetDistance = maxDistance;
            targetPosition.position = transform.position + worldRotationDirection * targetDistance;

            // Rotate the player based on the drag direction, unless there's an enemy
            RotatePlayer(worldRotationDirection);
        }
    }

    
    // Function to move the object to the closest possible position within max distance
    private void MoveToClosestPosition(Vector3 destination)
    {
        // Calculate the direction towards the destination from the current position
        Vector3 direction = (destination - transform.position).normalized;

        // Clamp the distance to ensure the object doesn't move beyond maxDistance
        float clampedDistance = Mathf.Min(maxDistance/2, maxDistance);

        // Calculate the final position based on the clamped distance
        targetPosition.transform.position = transform.position + direction * clampedDistance;
    }

    public void SetPosition(Vector3 position)
    {
        rb.MovePosition(position);
    }

    private void MovePlayer()
    {
        if (movementDirection.magnitude > 0)
        {
            rb.MovePosition(rb.position + movementDirection * (moveSpeed * Time.fixedDeltaTime));
        }
    }

private void RotatePlayer(Vector3 direction)
{
    // Find the closest enemy using the TargetingSystem
    Transform closestEnemy = targetingSystem.GetClosestEnemy(transform.position);

    // If there is a closest enemy, rotate towards it
    if (closestEnemy != null)
    {
        // Calculate the direction towards the closest enemy
        Vector3 directionToEnemy = (closestEnemy.position - transform.position).normalized;
        directionToEnemy.y = 0f;  // Ensure rotation is only on the XZ plane

        // Smoothly rotate towards the enemy
        SmoothRotation(directionToEnemy);
        
        rotationDirection = directionToEnemy;
    }
    else
    {
        // If no enemy is found, rotate based on the input direction
        if (direction.magnitude > 0)
        {
            // Smoothly rotate towards the input direction
            SmoothRotation(direction);
            
            rotationDirection = direction;
        }
    }
}

// Smooth rotation helper function
private void SmoothRotation(Vector3 direction)
{
    // Calculate the target rotation
    Quaternion targetRotation = Quaternion.LookRotation(direction);

    // Smoothly rotate using Slerp (smooth spherical interpolation)
    Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed);  // Adjust the speed multiplier as needed

    // Apply the smoothed rotation
    rb.MoveRotation(smoothedRotation);
}

    // Set movement speed from external source (PlayerController)
    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = newMoveSpeed;
    }
}