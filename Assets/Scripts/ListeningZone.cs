using UnityEngine;

public class ListeningZone : MonoBehaviour
{
    private int playersInside = 0;

    public bool HasPlayerInside => playersInside > 0;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playersInside++;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playersInside = Mathf.Max(0, playersInside - 1);
    }
}
