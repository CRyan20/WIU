using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Key,
    // Add more item types as needed
}


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemIcon; // The sprite representing the item
    public GameObject itemPrefab;
    public string description;
    public ItemType itemType;
    // Add more fields as needed for your specific item data

    [Range(1, 999)]
    public int maxStack = 1;

    public int AmmoCount;
    public int MaxAmmoCount;
}
