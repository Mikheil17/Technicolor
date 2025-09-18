using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CameraForwardMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform moveRoot; // should be XR Rig root (leave blank to use this transform)
    public float moveSpeed = 1.5f;
    public bool startMovingOnStart = true;

    [Header("Distance Limits (Optional)")]
    public bool useDistanceLimit = false;
    public float maxDistance = 10.0f;

    [Header("Turn Settings")]
    public bool enableTurn = true;
    public float turnAtX = 10f;             // trigger position (world X)
    public float turnAngle = -90f;          // left (-) or right (+)
    public float turnSmoothDuration = 0.5f; // seconds (0 = instant)

    [Header("Pause / Stop Settings")]
    public float pauseZ = 16f;    // world Z where we pause
    public float pauseDuration = 3f;
    public float stopZ = 20f;     // world Z where we stop for good

    private CharacterController controller;
    private bool hasTurned = false;
    private bool isMoving = false;
    private Vector3 startPosition;
    private float distanceTraveled = 0f;

    private bool hasPausedAtZ = false;
    private bool hasStoppedAtZ = false;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (moveRoot == null) moveRoot = transform; // fallback
    }

    void Start()
    {
        startPosition = moveRoot.position;
        if (startMovingOnStart) StartMoving();
    }

    void LateUpdate()
    {
        // take snapshot of position before movement
        float beforeX = moveRoot.position.x;
        float beforeZ = moveRoot.position.z;

        if (isMoving) MoveForward();

        // snapshot after movement
        float afterX = moveRoot.position.x;
        float afterZ = moveRoot.position.z;

        // check turn using before/after to avoid missing due to overshoot
        if (enableTurn && !hasTurned)
            CheckAndDoTurn(beforeX, afterX);

        // check pause (once) using before/after
        if (!hasPausedAtZ)
            CheckAndPause(beforeZ, afterZ);

        // check full stop at stopZ (immediate)
        if (!hasStoppedAtZ && afterZ >= stopZ)
        {
            hasStoppedAtZ = true;
            StopMoving();
        }
    }

    void MoveForward()
    {
        Vector3 motion = moveRoot.forward * moveSpeed * Time.deltaTime;
        controller.Move(motion);

        if (useDistanceLimit)
        {
            distanceTraveled += motion.magnitude;
            if (distanceTraveled >= maxDistance) StopMoving();
        }
    }

    void CheckAndPause(float beforeZ, float afterZ)
    {
        bool crossedForward = (beforeZ < pauseZ && afterZ >= pauseZ);
        bool crossedBackward = (beforeZ > pauseZ && afterZ <= pauseZ);

        if ((crossedForward || crossedBackward) && !hasPausedAtZ)
        {
            hasPausedAtZ = true;
            StartCoroutine(PauseAtZ());
        }
    }

    IEnumerator PauseAtZ()
    {
        StopMoving();
        yield return new WaitForSecondsRealtime(pauseDuration);
        // only resume if we haven't already reached the final stop Z
        if (!hasStoppedAtZ && moveRoot.position.z < stopZ)
            StartMoving();
    }

    void CheckAndDoTurn(float beforeX, float afterX)
    {
        bool crossedForward = (beforeX < turnAtX && afterX >= turnAtX);
        bool crossedBackward = (beforeX > turnAtX && afterX <= turnAtX);

        if (crossedForward || crossedBackward)
        {
            hasTurned = true;

            if (turnSmoothDuration <= 0f)
                moveRoot.Rotate(Vector3.up, turnAngle, Space.World);
            else
                StartCoroutine(SmoothTurnCoroutine(turnAngle, turnSmoothDuration));
        }
    }

    IEnumerator SmoothTurnCoroutine(float yAngle, float duration)
    {
        Quaternion startRot = moveRoot.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, yAngle, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            moveRoot.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        moveRoot.rotation = endRot;
    }

    // --- Public API ---
    public void StartMoving() => isMoving = true;
    public void StopMoving() => isMoving = false;
    public void ToggleMovement() => isMoving = !isMoving;

    public void ResetPosition()
    {
        controller.enabled = false;
        moveRoot.position = startPosition;
        controller.enabled = true;

        distanceTraveled = 0f;
        hasTurned = false;
        hasPausedAtZ = false;
        hasStoppedAtZ = false;
    }

    public void SetSpeed(float newSpeed) => moveSpeed = newSpeed;
}
