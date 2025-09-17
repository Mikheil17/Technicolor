using System.Collections;
using UnityEngine;

public class CameraForwardMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.0f;
    public bool startMovingOnStart = true;
    public bool smoothMovement = true;

    [Header("Distance Limits (Optional)")]
    public bool useDistanceLimit = false;
    public float maxDistance = 10.0f;

    private bool isMoving = false;
    private Vector3 startPosition;
    private float distanceTraveled = 0f;

    void Start()
    {
        startPosition = transform.position;

        if (startMovingOnStart)
        {
            StartMoving();
        }
    }

    void Update()
    {
        if (isMoving)
        {
            MoveForward();
        }
    }

    void MoveForward()
    {
        // Calculate movement this frame
        float moveThisFrame = moveSpeed * Time.deltaTime;

        // Apply smooth movement if enabled
        if (smoothMovement)
        {
            transform.Translate(Vector3.forward * moveThisFrame);
        }
        else
        {
            transform.position += transform.forward * moveThisFrame;
        }

        // Track distance if limit is enabled
        if (useDistanceLimit)
        {
            distanceTraveled += moveThisFrame;
            if (distanceTraveled >= maxDistance)
            {
                StopMoving();
            }
        }
    }

    public void StartMoving()
    {
        isMoving = true;
        Debug.Log("Camera started moving forward");
    }

    public void StopMoving()
    {
        isMoving = false;
        Debug.Log("Camera stopped moving");
    }

    public void ToggleMovement()
    {
        if (isMoving)
            StopMoving();
        else
            StartMoving();
    }

    public void ResetPosition()
    {
        transform.position = startPosition;
        distanceTraveled = 0f;
        Debug.Log("Camera position reset");
    }

    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
}