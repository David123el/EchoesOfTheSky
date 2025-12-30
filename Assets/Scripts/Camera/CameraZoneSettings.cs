using UnityEngine;

[System.Serializable]
public class CameraZoneSettings
{
    public Vector2 offset = Vector2.zero;
    public float smoothSpeed = 6f;

    public bool enableLookAhead = true;
    public float lookAheadDistance = 1.5f;
    public float lookAheadSmoothing = 4f;

    public bool overrideZoom = false;
    public float zoom = 5f;
    public float zoomSpeed = 3f;
}
