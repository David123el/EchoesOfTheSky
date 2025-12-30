using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxInfiniteX : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxFactor = 0.6f;

    private Transform cam;
    private float spriteWidth;

    private Vector3 logicalPosition;   // 🎯 מיקום לוגי (לא משתנה בפתאומיות)
    private float lastCamX;

    void Start()
    {
        cam = Camera.main.transform;
        spriteWidth = GetComponent<SpriteRenderer>().bounds.size.x;

        logicalPosition = transform.position;
        lastCamX = cam.position.x;
    }

    void LateUpdate()
    {
        float camX = cam.position.x;
        float deltaCamX = camX - lastCamX;

        // 🔁 מזיזים לוגית רק כשצריך
        float diff = camX - logicalPosition.x;

        if (diff > spriteWidth)
            logicalPosition.x += spriteWidth * 3f;
        else if (diff < -spriteWidth)
            logicalPosition.x -= spriteWidth * 3f;

        // 🎨 פרלקס ויזואלי בלבד
        float visualX = logicalPosition.x + deltaCamX * parallaxFactor;

        transform.position = new Vector3(
            visualX,
            transform.position.y,
            transform.position.z
        );

        lastCamX = camX;
    }
}
