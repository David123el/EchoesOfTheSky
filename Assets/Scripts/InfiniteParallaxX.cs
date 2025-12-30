using UnityEngine;

public class InfiniteParallaxX : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxFactor = 0.5f;

    public float preJumpOffset = 0.1f;

    private Transform cam;
    private Transform[] tiles;

    private float spriteWidth;
    private float lastCamX;

    [SerializeField] private PlayerMovement playerMovement;

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
        float camDelta = cam.position.x - lastCamX;
        lastCamX = cam.position.x;

        // === PARALLAX (יחסי, לא מצטבר) ===
        transform.position += Vector3.right * camDelta * parallaxFactor;

        HandleInfiniteLoop();
    }

    void HandleInfiniteLoop()
    {
        float camX = cam.position.x;

        foreach (Transform tile in tiles)
        {
            float distance = tile.position.x - camX;

            if (playerMovement.isMovingRight)
            {
                if (distance < -spriteWidth - preJumpOffset)
                {
                    tile.position += Vector3.right * spriteWidth * 3;
                }
            }
            else if (playerMovement.isMovingRight == false)
            {
                if (distance > spriteWidth + preJumpOffset)
                {
                    tile.position -= Vector3.right * spriteWidth * 3;
                }
            }
        }
    }
}
