using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public GameObject[] keys; // Array to hold the key GameObjects
    private int keysCollected = 0; // Counter to keep track of collected keys
    private bool isOpen = false; // Flag to check if the chest is already open

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item")) // Check if the collider belongs to a key GameObject
        {
            // Deactivate the collected key GameObject
            other.gameObject.SetActive(false);
            keysCollected++; // Increase the collected keys count

            if (keysCollected >= 4 && !isOpen) // Check if all keys are collected and the chest is not already open
            {
                OpenChest(); // Call the method to open the chest
            }
        }
    }

    void OpenChest()
    {
        // Add code here to perform any actions when the chest is opened
        Debug.Log("Treasure chest opened!");
        isOpen = true; // Set the flag to indicate that the chest is now open
    }
}
