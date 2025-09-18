using System.Collections;
using UnityEngine;

public class CameraForwardMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform moveTransform;           // the object that actually moves (set to XR Rig root if using VR)
    public float moveSpeed = 1.0f;
    public bool startMovingOnStart = true;

    [Header("Distance Limits (Optional)")]
    public bool useDistanceLimit = false;
    public float maxDistance = 10.0f;

    [Header("Turn Settings")]
    public bool enableTurn = true;
    public float turnAtX = 10f;        // world X coordinate where the turn should trigger
    public float turnAngle = -90f;       // negative = left, positive = right
    public float turnSmoothDuration = 0.5f; // seconds to interpolate rotation (0 = instant)

    private bool hasTurned = false;      // ensure we only trigger once
    private bool isMoving = false;
    private Vector3 startPosition;
    private float distanceTraveled = 0f;
    private float prevX;

    void Start()
    {
        if (moveTransform == null) moveTransform = transform; // fallback to this object

        startPosition = moveTransform.position;
        prevX = moveTransform.position.x;

        if (startMovingOnStart) StartMoving();
    }

    void Update()
    {
        if (isMoving)
        {
            MoveForward();
        }

        if (enableTurn && !hasTurned)
        {
            CheckAndDoTurn();
        }

        // update prevX for next frame
        prevX = moveTransform.position.x;
    }

    void MoveForward()
    {
        float moveThisFrame = moveSpeed * Time.deltaTime;
        moveTransform.position += moveTransform.forward * moveThisFrame;

        if (useDistanceLimit)
        {
            distanceTraveled += moveThisFrame;
            if (distanceTraveled >= maxDistance)
            {
                StopMoving();
            }
        }
    }

    void CheckAndDoTurn()
    {
        float curX = moveTransform.position.x;

        // Crossing test: supports movement in either +X or -X direction
        bool crossedForward = (prevX < turnAtX && curX >= turnAtX);
        bool crossedBackward = (prevX > turnAtX && curX <= turnAtX);

        if (crossedForward || crossedBackward)
        {
            Debug.Log($"Turn triggered at X={curX:F3} (prevX={prevX:F3}). Starting turn.");
            hasTurned = true;
            if (turnSmoothDuration <= 0f)
            {
                // instant
                moveTransform.Rotate(Vector3.up, turnAngle, Space.World);
            }
            else
            {
                StartCoroutine(SmoothTurnCoroutine(turnAngle, turnSmoothDuration));
            }
        }
    }

    IEnumerator SmoothTurnCoroutine(float yAngle, float duration)
    {
        Quaternion startRot = moveTransform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, yAngle, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
            moveTransform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        moveTransform.rotation = endRot;
        Debug.Log("Turn complete. New forward: " + moveTransform.forward);
    }

    // Control API
    public void StartMoving() { isMoving = true; }
    public void StopMoving() { isMoving = false; }
    public void ToggleMovement() { isMoving = !isMoving; }

    public void ResetPosition()
    {
        moveTransform.position = startPosition;
        distanceTraveled = 0f;
        hasTurned = false;
        prevX = moveTransform.position.x;
        Debug.Log("Camera position reset");
    }

    public void SetSpeed(float newSpeed) { moveSpeed = newSpeed; }
}
