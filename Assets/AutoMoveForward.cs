using UnityEngine;

public class AutoMoveForward : MonoBehaviour
{
    public float speed = 1.5f; // movement speed (meters per second)

    void Update()
    {
        // Move forward along the object's local forward direction
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}