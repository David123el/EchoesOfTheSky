using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Settings")]
    public float smoothSpeed = 8f;
    public Vector2 offset;

    [Header("Look Ahead")]
    public bool enableLookAhead = true;
    public float lookAheadDistance = 1.5f;
    public float lookAheadSmoothing = 4f;
    public float minVelocityForLookAhead = 0.1f;

    [Header("Bounds")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Vector2 lookAhead;
    private Rigidbody2D targetRb;
    private Camera cam;

    [Header("Default Zone Settings")]
    public CameraZoneSettings defaultSettings;

    void Awake()
    {
        if (target != null)
            targetRb = target.GetComponent<Rigidbody2D>();

        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = target.position + (Vector3)offset;

        // Look Ahead
        if (enableLookAhead && targetRb != null)
        {
            float velocityX = targetRb.linearVelocity.x;

            if (Mathf.Abs(velocityX) > minVelocityForLookAhead)
            {
                float direction = Mathf.Sign(velocityX);
                Vector2 desiredLookAhead = Vector2.right * direction * lookAheadDistance;
                lookAhead = Vector2.Lerp(lookAhead, desiredLookAhead, Time.deltaTime * lookAheadSmoothing);
            }
            else
            {
                lookAhead = Vector2.Lerp(lookAhead, Vector2.zero, Time.deltaTime * lookAheadSmoothing);
            }

            targetPos += (Vector3)lookAhead;
        }

        Vector3 smoothPos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);

        // Bounds
        if (useBounds && cam != null)
        {
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = camHalfHeight * cam.aspect;

            smoothPos.x = Mathf.Clamp(smoothPos.x, minBounds.x + camHalfWidth, maxBounds.x - camHalfWidth);
            smoothPos.y = Mathf.Clamp(smoothPos.y, minBounds.y + camHalfHeight, maxBounds.y - camHalfHeight);
        }

        smoothPos.z = transform.position.z;
        transform.position = smoothPos;
    }

    private void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube((minBounds + maxBounds) / 2f, maxBounds - minBounds);
        }
    }

    public void ApplyZoneSettings(CameraZoneSettings settings)
    {
        offset = settings.offset;
        smoothSpeed = settings.smoothSpeed;

        enableLookAhead = settings.enableLookAhead;
        lookAheadDistance = settings.lookAheadDistance;
        lookAheadSmoothing = settings.lookAheadSmoothing;

        if (settings.overrideZoom && cam != null)
        {
            StopAllCoroutines();
            StartCoroutine(ZoomTo(settings.zoom, settings.zoomSpeed));
        }
    }

    private System.Collections.IEnumerator ZoomTo(float targetZoom, float speed)
    {
        while (Mathf.Abs(cam.orthographicSize - targetZoom) > 0.01f)
        {
            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                targetZoom,
                Time.deltaTime * speed
            );
            yield return null;
        }

        cam.orthographicSize = targetZoom;
    }

    public void ResetToDefault()
    {
        ApplyZoneSettings(defaultSettings);
    }
}
