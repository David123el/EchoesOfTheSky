using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("1 = רחוק (זז כמעט עם המצלמה), 0 = קרוב (כמעט לא זז)")]
    [Range(0f, 1f)]
    public float depth = 1f;

    private Transform cam;
    private Vector3 startPos;

    void Start()
    {
        cam = Camera.main.transform;
        startPos = transform.position;
    }

    void LateUpdate()
    {
        Vector3 camOffset = cam.position;

        // 🔑 עומק גבוה = רחוק = תנועה מלאה
        transform.position = new Vector3(
            startPos.x + camOffset.x * depth,
            startPos.y + camOffset.y * depth,
            startPos.z
        );
    }
}
