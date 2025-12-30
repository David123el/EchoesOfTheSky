using UnityEngine;

public class SoundShard : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        SoundShardManager.Instance.CollectShard(transform.position);

        Destroy(gameObject, 0.05f);
    }

    public string GetPromptText() => "Collect";
}
