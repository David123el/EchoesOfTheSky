using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class WindowLight : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Color litColor = new Color(1f, 0.95f, 0.8f);
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float maxAlpha = 1f;

    [Header("Behavior")]
    [SerializeField] private float appearSmooth = 6f;
    [SerializeField] private bool randomOffset = true;

    private SpriteRenderer sr;
    private float targetAlpha;
    private float currentAlpha;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        if (randomOffset)
            currentAlpha = Random.Range(0f, 0.2f);

        SetAlpha(0f);
    }

    void Update()
    {
        currentAlpha = Mathf.Lerp(
            currentAlpha,
            targetAlpha,
            Time.deltaTime * appearSmooth
        );

        SetAlpha(currentAlpha);
    }

    public void ApplyAwakening(float intensity)
    {
        targetAlpha = Mathf.Lerp(minAlpha, maxAlpha, intensity);
    }

    private void SetAlpha(float a)
    {
        Color c = litColor;
        c.a = a;
        sr.color = c;
    }
}
