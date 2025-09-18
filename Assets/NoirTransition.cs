using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using EasyDoorSystem; // 👈 make sure we can see EasyDoor

public class SimpleColorTransition : MonoBehaviour
{
    [Header("Post-Processing Volume")]
    public PostProcessVolume postProcessVolume;

    [Header("Settings")]
    public float transitionSpeed = 1.0f;

    [Header("Door Reference")]
    public EasyDoor door;   // drag your door with EasyDoor script here in Inspector

    private ColorGrading colorGrading;
    private Bloom bloom;
    private Grain grain;

    private bool isTransitioning = false;

    void Start()
    {
        // Get post-processing effects
        postProcessVolume.profile.TryGetSettings(out colorGrading);
        postProcessVolume.profile.TryGetSettings(out bloom);
        postProcessVolume.profile.TryGetSettings(out grain);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            // 🔑 Tell EasyDoor to open
            if (door != null)
            {
                Debug.Log("Trigger entered: opening EasyDoor.");
                door.OpenDoor();
            }

            // Start color transition
            StartCoroutine(TransitionToColor());
        }
    }

    IEnumerator TransitionToColor()
    {
        isTransitioning = true;
        float timer = 0f;

        if (grain != null)
            grain.enabled.value = false;

        while (timer < 1f)
        {
            timer += Time.deltaTime * transitionSpeed;
            float t = Mathf.SmoothStep(0f, 1f, timer);

            if (colorGrading != null)
                colorGrading.saturation.value = Mathf.Lerp(-50f, 100f, t);

            if (bloom != null)
                bloom.intensity.value = Mathf.Lerp(75f, 3f, t);

            yield return null;
        }

        isTransitioning = false;
    }
}
