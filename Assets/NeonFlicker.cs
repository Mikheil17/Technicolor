using System.Collections;
using UnityEngine;


public class NeonFlicker : MonoBehaviour
{
    // The SpriteRenderer component to change the color.
    private SpriteRenderer spriteRenderer;

    // The AudioSource component to play the sound effect from.
    public AudioSource audioSource;

    // The two colors to flicker between.
    // The 'flickerColor' is the "on" color, which is white in this case.
    public Color offColor = Color.black;
    public Color onColor = Color.white;

    // The minimum and maximum time between flickers.
    public float minFlickerInterval = 0.05f;
    public float maxFlickerInterval = 0.2f;

    // The minimum and maximum time the light stays off.
    public float minOffTime = 0.5f;
    public float maxOffTime = 1.5f;

    // The sound to play when the light flickers.
    public AudioClip flickerSound;

    // Flag to control the flickering effect.
    private bool isFlickering = false;

    // This method is called when the script instance is being loaded.
    void Awake()
    {
        // Get the SpriteRenderer component attached to this GameObject.
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If no AudioSource is assigned, try to find one on this GameObject.
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }


    void Start()
    {
        // Start the flickering coroutine.
        if (!isFlickering)
        {
            StartCoroutine(FlickerRoutine());
        }
    }

    // This coroutine handles the flickering effect.
    private IEnumerator FlickerRoutine()
    {
        isFlickering = true;

        while (true)
        {
            // Set the color to the "off" state (black).
            spriteRenderer.color = offColor;

            // Wait for a random amount of time.
            yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));

            // Flicker a few times.
            int flickerCount = Random.Range(3, 7);
            for (int i = 0; i < flickerCount; i++)
            {
                // Play the sound effect.
                if (audioSource != null && flickerSound != null)
                {
                    audioSource.PlayOneShot(flickerSound);
                }

                // Switch to the "on" color (white/red).
                spriteRenderer.color = onColor;

                // Wait for a short, random time.
                yield return new WaitForSeconds(Random.Range(minFlickerInterval, maxFlickerInterval));

                // Switch back to the "off" color.
                spriteRenderer.color = offColor;

                // Wait for a short, random time.
                yield return new WaitForSeconds(Random.Range(minFlickerInterval, maxFlickerInterval));
            }

            // Set the color to the "on" state for a moment.
            spriteRenderer.color = onColor;
            yield return new WaitForSeconds(Random.Range(minFlickerInterval, maxFlickerInterval) * 2);
        }
    }

    // Stop the coroutine if the object is disabled.
    void OnDisable()
    {
        StopAllCoroutines();
        isFlickering = false;
    }
}
