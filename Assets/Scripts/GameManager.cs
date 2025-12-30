using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Dictionary<ItemType, int> inventory = new Dictionary<ItemType, int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(ItemType type, int amount)
    {
        if (!inventory.ContainsKey(type))
            inventory[type] = 0;

        inventory[type] += amount;

        Debug.Log($"{type} collected. Total: {inventory[type]}");
    }

    public int GetItemCount(ItemType type)
    {
        return inventory.ContainsKey(type) ? inventory[type] : 0;
    }
}
