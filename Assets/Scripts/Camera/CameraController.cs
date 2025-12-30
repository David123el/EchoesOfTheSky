using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.U2D;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(PixelPerfectCamera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Pulse Settings")]
    [SerializeField] private float stepInterval = 0.015f; // מהירות הקפיצה
    [SerializeField] private int stepSize = 1;            // כמה PPU בכל צעד

    private PixelPerfectCamera pixelCam;
    private int defaultPPU;
    private Coroutine pulseRoutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        pixelCam = GetComponent<PixelPerfectCamera>();
        defaultPPU = pixelCam.assetsPPU;
    }

    public void PulseZoom(int ppuOffset, float holdTime)
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(PulseRoutine(ppuOffset, holdTime));
    }

    private System.Collections.IEnumerator PulseRoutine(int ppuOffset, float holdTime)
    {
        int targetPPU = Mathf.Max(1, defaultPPU - ppuOffset);

        // 🔽 זום אין – ירידה מדורגת
        while (pixelCam.assetsPPU > targetPPU)
        {
            pixelCam.assetsPPU -= stepSize;
            yield return new WaitForSeconds(stepInterval);
        }

        // ⏸ החזקה קצרה
        yield return new WaitForSeconds(holdTime);

        // 🔼 חזרה מדורגת
        while (pixelCam.assetsPPU < defaultPPU)
        {
            pixelCam.assetsPPU += stepSize;
            yield return new WaitForSeconds(stepInterval);
        }

        // 🔒 נעילה לערך המדויק
        pixelCam.assetsPPU = defaultPPU;
    }
}
