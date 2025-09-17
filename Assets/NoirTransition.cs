using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class NoirTransition : MonoBehaviour
{
    [Header("Post-Processing Volume")]
    public PostProcessVolume postProcessVolume;

    [Header("Noir Settings")]
    public float noirGrain = 1.0f;
    public float noirVignette = 0.5f;
    public float noirBloom = 0.1f;

    [Header("Color Settings")]
    public float colorGrain = 0.1f;
    public float colorVignette = 0.0f;
    public float colorBloom = 1.0f;

    [Header("Transition")]
    public float transitionSpeed = 2.0f;

    private Grain filmGrain;
    private Vignette vignette;
    private Bloom bloom;

    private bool inColorZone = false;
    private bool isTransitioning = false;

    void Start()
    {
        // Try to get settings
        if (postProcessVolume.profile.TryGetSettings<Grain>(out filmGrain))
        {
            Debug.Log("✓ Grain found");
            SetNoirMode();
        }
        else Debug.Log("✗ Grain NOT found");

        if (postProcessVolume.profile.TryGetSettings<Vignette>(out vignette))
            Debug.Log("✓ Vignette found");
        else Debug.Log("✗ Vignette NOT found");

        if (postProcessVolume.profile.TryGetSettings<Bloom>(out bloom))
            Debug.Log("✓ Bloom found");
        else Debug.Log("✗ Bloom NOT found");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter: " + other.name + " Tag=" + other.tag);

        if (other.CompareTag("Player") && !isTransitioning)
        {
            Debug.Log(">> Starting transition to COLOR");
            inColorZone = true;
            StartCoroutine(TransitionToColor());
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exit: " + other.name + " Tag=" + other.tag);

        if (other.CompareTag("Player") && !isTransitioning)
        {
            Debug.Log(">> Starting transition back to NOIR");
            inColorZone = false;
            StartCoroutine(TransitionToNoir());
        }
    }

    IEnumerator TransitionToColor()
    {
        isTransitioning = true;
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * transitionSpeed;
            float t = Mathf.SmoothStep(0f, 1f, timer);

            if (filmGrain != null)
            {
                filmGrain.intensity.value = Mathf.Lerp(noirGrain, colorGrain, t);
                Debug.Log($"Grain={filmGrain.intensity.value}");
            }

            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(noirVignette, colorVignette, t);
                Debug.Log($"Vignette={vignette.intensity.value}");
            }

            if (bloom != null)
            {
                bloom.intensity.value = Mathf.Lerp(noirBloom, colorBloom, t);
                Debug.Log($"Bloom={bloom.intensity.value}");
            }

            yield return null;
        }

        Debug.Log(">> Transition to COLOR complete");
        isTransitioning = false;
    }

    IEnumerator TransitionToNoir()
    {
        isTransitioning = true;
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * transitionSpeed;
            float t = Mathf.SmoothStep(0f, 1f, timer);

            if (filmGrain != null)
            {
                filmGrain.intensity.value = Mathf.Lerp(colorGrain, noirGrain, t);
                Debug.Log($"Grain={filmGrain.intensity.value}");
            }

            if (vignette != null)
            {
                vignette.intensity.value = Mathf.Lerp(colorVignette, noirVignette, t);
                Debug.Log($"Vignette={vignette.intensity.value}");
            }

            if (bloom != null)
            {
                bloom.intensity.value = Mathf.Lerp(colorBloom, noirBloom, t);
                Debug.Log($"Bloom={bloom.intensity.value}");
            }

            yield return null;
        }

        Debug.Log(">> Transition to NOIR complete");
        isTransitioning = false;
    }

    void SetNoirMode()
    {
        if (filmGrain != null) filmGrain.intensity.value = noirGrain;
        if (vignette != null) vignette.intensity.value = noirVignette;
        if (bloom != null) bloom.intensity.value = noirBloom;
        Debug.Log("Initial noir mode set: Grain=" + noirGrain + " Vignette=" + noirVignette + " Bloom=" + noirBloom);
    }
}
