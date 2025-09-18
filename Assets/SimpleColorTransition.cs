using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SimpleColorTransition : MonoBehaviour
{
    [Header("Post-Processing Volume")]
    public PostProcessVolume postProcessVolume;

    [Header("Timer Settings")]
    public float delayBeforeTransition = 15.0f;

    [Header("Transition Settings")]
    public float transitionSpeed = 2.0f;

    private ColorGrading colorGrading;
    private Bloom bloom;
    private Vignette vignette;
    private bool isTransitioning = false;

    void Start()
    {
        // Get post-processing effects
        postProcessVolume.profile.TryGetSettings<ColorGrading>(out colorGrading);
        postProcessVolume.profile.TryGetSettings<Bloom>(out bloom);
        postProcessVolume.profile.TryGetSettings<Vignette>(out vignette);

        // Set starting values (intense noir mode)
        if (colorGrading != null)
        {
            colorGrading.saturation.value = -100f; // Complete black and white
            colorGrading.contrast.value = 25f; // High contrast for dramatic shadows
            colorGrading.temperature.value = -15f; // Cool/blue tone
        }
        if (bloom != null)
            bloom.intensity.value = 0.05f; // Minimal bloom
        if (vignette != null)
        {
            vignette.intensity.value = 0.7f; // Very strong vignette
            vignette.smoothness.value = 0.2f; // Harsh vignette edges
        }

        // Start the timer for automatic transition
        StartCoroutine(TimedColorTransition());
    }

    System.Collections.IEnumerator TimedColorTransition()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeTransition);

        // Start the color transition
        yield return StartCoroutine(TransitionToColor());
    }

    System.Collections.IEnumerator TransitionToColor()
    {
        isTransitioning = true;
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * transitionSpeed;
            float t = Mathf.SmoothStep(0f, 1f, timer);

            // Change saturation from -100 (complete B&W) to +75 (super vibrant)
            if (colorGrading != null)
            {
                colorGrading.saturation.value = Mathf.Lerp(-100f, 75f, t);
                colorGrading.contrast.value = Mathf.Lerp(25f, 10f, t); // Reduce harsh contrast
                colorGrading.temperature.value = Mathf.Lerp(-15f, 5f, t); // Warm up the colors
            }

            // Increase bloom dramatically from 0.05 to 3.0 (very high)
            if (bloom != null)
                bloom.intensity.value = Mathf.Lerp(0.05f, 3.0f, t);

            // Reduce vignette from 0.7 (very strong) to 0.05 (almost none)
            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(0.7f, 0.05f, t);
                vignette.smoothness.value = Mathf.Lerp(0.2f, 0.4f, t); // Softer edges
            }

            yield return null;
        }

        isTransitioning = false;
    }
}