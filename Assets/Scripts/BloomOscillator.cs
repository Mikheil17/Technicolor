using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BloomOscillator : MonoBehaviour
{
    [Header("Post-Processing Volume")]
    public PostProcessVolume postProcessVolume;

    [Header("Bloom Oscillation Settings")]
    public float minBloom = 0.1f;
    public float maxBloom = 2.0f;
    public float oscillationSpeed = 1.0f; // How fast it goes up and down

    private Bloom bloom;

    void Start()
    {
        // Get the bloom effect from the post-processing profile
        postProcessVolume.profile.TryGetSettings<Bloom>(out bloom);

        if (bloom == null)
        {
            Debug.LogWarning("Bloom effect not found in the Post-Processing Profile!");
        }
    }

    void Update()
    {
        if (bloom != null)
        {
            // Use sine wave to create smooth oscillation between min and max values
            float oscillation = Mathf.Sin(Time.time * oscillationSpeed);

            // Convert from -1 to 1 range to minBloom to maxBloom range
            float bloomValue = Mathf.Lerp(minBloom, maxBloom, (oscillation + 1f) / 2f);

            // Apply the bloom value
            bloom.intensity.value = bloomValue;
        }
    }
}