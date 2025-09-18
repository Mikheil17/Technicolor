using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SimpleColorTransition : MonoBehaviour
{
    [Header("Post-Processing Volume")]
    public PostProcessVolume postProcessVolume;

    [Header("Settings")]
    public float transitionSpeed = 2.0f;

    private ColorGrading colorGrading;
    private Bloom bloom;
    private Grain grain;

    private bool isTransitioning = false;

    void Start()
    {
        // Get post-processing effects
        postProcessVolume.profile.TryGetSettings<ColorGrading>(out colorGrading);
        postProcessVolume.profile.TryGetSettings<Bloom>(out bloom);
        postProcessVolume.profile.TryGetSettings<Grain>(out grain);

        // Set starting values (noir mode)
        if (colorGrading != null)
            colorGrading.saturation.value = -50f; // Black and white

        if (bloom != null)
            bloom.intensity.value = 0.1f; // Low bloom

        // Disable grain completely
        if (grain != null)
            grain.enabled.value = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(TransitionToColor());
        }
    }

    System.Collections.IEnumerator TransitionToColor()
    {
        isTransitioning = true;
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * transitionSpeed;
            float t = Mathf.SmoothStep(0f, 1f, timer);

            // Change saturation from -50 (B&W) to +50 (vibrant)
            if (colorGrading != null)
                colorGrading.saturation.value = Mathf.Lerp(-50f, 50f, t);

            // Increase bloom from 0.1 to 5.0 (very strong and bright)
            if (bloom != null)
                bloom.intensity.value = Mathf.Lerp(0.1f, 5.0f, t);

            yield return null;
        }

        isTransitioning = false;
    }
}