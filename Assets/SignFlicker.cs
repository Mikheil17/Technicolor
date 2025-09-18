using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlicker : MonoBehaviour
{
    public Color baseColor = Color.cyan;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float flickerSpeed = 10.0f;

    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Perlin noise for smooth random flicker
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0.0f);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        sprite.color = baseColor * intensity;  // Multiplies brightness

        // Debug check
        // Debug.Log($"Flicker intensity: {intensity:F2}");
    }
}
