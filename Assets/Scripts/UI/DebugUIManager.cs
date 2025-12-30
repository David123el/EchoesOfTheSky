using UnityEngine;
using TMPro;

public class DebugUIManager : MonoBehaviour
{
    public static DebugUIManager Instance;

    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private float messageDuration = 2f;

    private float timer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        debugText.text = "";
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                debugText.text = "";
        }
    }

    public void ShowMessage(string message)
    {
        debugText.text = message;
        timer = messageDuration;
    }
}
