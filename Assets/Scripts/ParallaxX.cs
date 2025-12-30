using UnityEngine;

public class ParallaxX : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    private Transform cam;
    private float startX;
    private float camStartX;

    void Start()
    {
        cam = Camera.main.transform;
        startX = transform.position.x;
        camStartX = cam.position.x;
    }

    void LateUpdate()
    {
        float cameraDelta = cam.position.x - camStartX;
        float newX = startX + cameraDelta * parallaxFactor;

        transform.position = new Vector3(
            newX,
            transform.position.y,
            transform.position.z
        );
    }
}
