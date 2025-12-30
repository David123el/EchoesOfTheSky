using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PixelPerfectCameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Offset")]
    public Vector2 offset;

    [Header("Look Ahead")]
    public bool enableLookAhead = true;
    public float lookAheadDistance = 1f;
    public float minVelocityForLookAhead = 0.1f;

    [Header("Bounds")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Rigidbody2D targetRb;
    private Camera cam;

    private int lastMoveDirection = 0; // 🔑 שמירת כיוון אחרון

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (target != null)
            targetRb = target.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 pos = target.position + (Vector3)offset;

        // 🧭 Look Ahead יציב
        if (enableLookAhead && targetRb != null)
        {
            float vx = targetRb.linearVelocity.x;

            if (Mathf.Abs(vx) > minVelocityForLookAhead)
            {
                lastMoveDirection = (int)Mathf.Sign(vx);
            }

            if (lastMoveDirection != 0)
            {
                pos.x += Mathf.Round(lookAheadDistance) * lastMoveDirection;
            }
        }

        // 🧱 Bounds
        if (useBounds)
        {
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;

            pos.x = Mathf.Clamp(pos.x, minBounds.x + halfW, maxBounds.x - halfW);
            pos.y = Mathf.Clamp(pos.y, minBounds.y + halfH, maxBounds.y - halfH);
        }

        pos.z = transform.position.z;
        transform.position = pos;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!useBounds) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            (minBounds + maxBounds) / 2f,
            maxBounds - minBounds
        );
    }
#endif
}
