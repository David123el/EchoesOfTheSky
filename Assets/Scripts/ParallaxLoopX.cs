using UnityEngine;

public class ParallaxLoopX : MonoBehaviour
{
    [Header("Parallax")]
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;
    // רחוק = 1 | קרוב = 0

    private Transform cam;
    private float spriteWidth;
    private Vector3 startPos;

    void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;
    }

    void LateUpdate()
    {
        // תזוזת פרלקס
        float deltaX = cam.position.x * parallaxFactor;
        transform.position = new Vector3(
            startPos.x + deltaX,
            startPos.y,
            startPos.z
        );

        // Loop אינסופי
        float diff = cam.position.x - transform.position.x;

        if (Mathf.Abs(diff) >= spriteWidth)
        {
            float offset = diff > 0 ? spriteWidth : -spriteWidth;
            startPos.x += offset;
        }
    }
}
