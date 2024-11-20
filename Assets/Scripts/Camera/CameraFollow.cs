using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float followSpeed = 5f;  // Speed at which the camera follows the player

    private Vector3 offset;          // Initial offset between the camera and the player
    private Vector3 velocity = Vector3.zero;  // SmoothDamp's velocity reference

    [SerializeField] private bool gotStart;
    [SerializeField] private AudioListener audioListener;


    private void Start()
    {
        if (PlayerReferenceManager.Instance.IsMobile) GetComponent<Camera>().orthographicSize -= 3;
    }

    private void FixedUpdate()
    {
        if (!gotStart && PlayerReferenceManager.Instance.playerController != null)
        {
            offset = transform.position - PlayerReferenceManager.Instance.PlayerTransform.position;
            gotStart = true;
        }
        
        if (!gotStart) return;

        // Get the target position (only X and Z axes, Y is fixed)
        Vector3 targetPosition = new Vector3(
            PlayerReferenceManager.Instance.PlayerTransform.position.x + offset.x,
            transform.position.y,
            PlayerReferenceManager.Instance.PlayerTransform.position.z + offset.z
        );

        // Smoothly follow the target position with SmoothDamp
        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1 / followSpeed);

        // Set the camera's position
        transform.position = smoothPosition;
    }
}