using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlicker : MonoBehaviour
{
    public Color litColor = Color.cyan;
    public Color unlitColor = Color.black;
    public float minOnTime = 0.05f;  // how short a flash can be
    public float maxOnTime = 0.3f;   // how long it can stay lit
    public float minOffTime = 0.1f;  // how short a dark period can be
    public float maxOffTime = 1.0f;  // how long it can stay off

    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Turn on
            sprite.color = litColor;
            yield return new WaitForSeconds(Random.Range(minOnTime, maxOnTime));

            // Turn off
            sprite.color = unlitColor;
            yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));
        }
    }
}
