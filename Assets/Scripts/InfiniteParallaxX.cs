using UnityEngine;

public class InfiniteParallaxX : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float parallaxFactor = 0.5f;

    [SerializeField] private float preloadOffset = 0.5f;

    //[SerializeField] private float cullDistance = 30f;

    private Transform cam;
    private Transform[] tiles;

    private float spriteWidth;
    private float lastCamX;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamX = cam.position.x;

        int count = transform.childCount;
        tiles = new Transform[count];

        for (int i = 0; i < count; i++)
            tiles[i] = transform.GetChild(i);

        spriteWidth = tiles[0]
            .GetComponent<SpriteRenderer>()
            .bounds.size.x;
    }

    void LateUpdate()
    {
        float camX = cam.position.x;
        float camDelta = camX - lastCamX;

        // ⛔ סינון רעידות מיקרו
        if (Mathf.Abs(camDelta) < 0.001f)
        {
            camDelta = 0f;
        }

        lastCamX = camX;

        // Parallax movement
        transform.position += Vector3.right * camDelta * parallaxFactor;

        HandleInfiniteLoop(camX);
    }

    void HandleInfiniteLoop(float camX)
    {
        foreach (Transform tile in tiles)
        {
            float distance = tile.position.x - camX;

            /*if (Mathf.Abs(distance) > cullDistance)
                continue;*/

            if (distance < -spriteWidth - preloadOffset)
            {
                tile.position += Vector3.right * spriteWidth * tiles.Length;
            }
            else if (distance > spriteWidth + preloadOffset)
            {
                tile.position -= Vector3.right * spriteWidth * tiles.Length;
            }
        }
    }
}
