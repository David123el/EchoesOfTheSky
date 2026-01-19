using UnityEngine;

[CreateAssetMenu(fileName = "EchoPlatformProfile", menuName = "Game/Echo Platform Profile")]
public class EchoPlatformProfile : ScriptableObject
{
    [Header("Timing")]
    public float disappearDelay = 0.8f;
    public float fadeDuration = 0.12f;

    [Header("Behavior")]
    public bool lockWhenStepped = false;
}
