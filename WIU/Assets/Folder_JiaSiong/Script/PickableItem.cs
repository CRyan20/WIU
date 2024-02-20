using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PickableItem : MonoBehaviour
{
    public ItemData itemData; // Reference to the ScriptableObject containing item data
    public Photon.Realtime.Player Owner;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager inventoryManager = other.GetComponent<InventoryManager>();
            if (inventoryManager != null)
            {
                // Store the viewID of the item
                int viewID = GetComponent<PhotonView>().ViewID;

                inventoryManager.TryPickupItem(viewID);
            }
        }
    }
}
