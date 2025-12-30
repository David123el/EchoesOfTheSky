using UnityEngine;

public class ListeningUI : MonoBehaviour
{
    public Transform player;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.08f;

    Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * (1f + pulse);
    }
}
