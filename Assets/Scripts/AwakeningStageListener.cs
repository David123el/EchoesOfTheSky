using UnityEngine;

public abstract class AwakeningStageListener : MonoBehaviour
{
    [SerializeField] protected int awakenStage = 1;
    private bool awakened = false;

    protected virtual void OnEnable()
    {
        WorldAwakeningManager.OnStageChanged += HandleStageChanged;
    }

    protected virtual void OnDisable()
    {
        WorldAwakeningManager.OnStageChanged -= HandleStageChanged;
    }

    private void HandleStageChanged(int stage)
    {
        if (awakened) return;
        if (stage >= awakenStage)
        {
            awakened = true;
            OnAwaken();
        }
    }

    protected abstract void OnAwaken();
}
