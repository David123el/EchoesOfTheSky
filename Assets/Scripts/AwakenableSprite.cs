using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AwakenableSprite : MonoBehaviour
{
    public Color baseColor;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color; // 🎯 שומר צבע ייחודי לכל אובייקט
    }

    public void ApplyAwakening(Color worldTint, float intensity)
    {
        Color final = Color.Lerp(
            Color.gray,               // מצב "ישן"
            baseColor * worldTint,    // צבע ער
            intensity
        );

        sr.color = new Color(
            final.r,
            final.g,
            final.b,
            sr.color.a
        );
    }
}
