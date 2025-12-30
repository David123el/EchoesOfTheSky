using UnityEngine;

public class WindReactiveObject : AwakeningStageListener
{
    [Header("Sway Base")]
    [SerializeField] private float maxSwayAngle = 8f;
    [SerializeField] private float swaySpeed = 1.2f;

    [Header("Fade In")]
    [SerializeField] private float fadeInDuration = 2f;

    private float activationProgress = 0f;
    private bool isActive = false;
    private Quaternion startRotation;

    private WorldAwakeningManager world;

    private void Awake()
    {
        startRotation = transform.localRotation;
        world = FindFirstObjectByType<WorldAwakeningManager>();
    }

    protected override void OnAwaken()
    {
        isActive = true;
        activationProgress = 0f;
    }

    private void Update()
    {
        if (!isActive || world == null) return;

        // Fade-in
        activationProgress = Mathf.MoveTowards(
            activationProgress,
            1f,
            Time.deltaTime / fadeInDuration
        );

        float windStrength = world.CurrentWindStrength;

        float sway =
            Mathf.Sin(Time.time * swaySpeed) *
            maxSwayAngle *
            windStrength *
            activationProgress;

        transform.localRotation =
            startRotation *
            Quaternion.Euler(0f, 0f, sway);
    }
}
