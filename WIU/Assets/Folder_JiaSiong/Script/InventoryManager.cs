using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public GameObject selectionIndicator;
    public GameObject invItemPrefab; // Prefab of the inventory item game object
    public GameObject[] inventorySlots;
    private int selectedSlotIndex = 0;
    [SerializeField]
    private bool pickupbool = false;
    private bool isNearItem = false; // Flag to track if the player is near an item
    public TextMeshProUGUI pickupText; // Reference to TextMeshProUGUI
    private Dictionary<int, PickableItem> inventoryItems = new Dictionary<int, PickableItem>();
    private GameObject[] invItemPrefabs;
    private bool isPickingUp = false; // Flag to prevent multiple pickups
    void Start()
    {
        invItemPrefabs = new GameObject[inventorySlots.Length];
    }
    void Update()
    {
        HandleSlotSelectionInput();

        // Check for key input to pick up and drop items
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isNearItem) // Check if the player is near an item before picking up
            {
                pickupbool = true;
                TryPickupItem();
            }
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            TryDropItem();
        }
        pickupbool = false;
    }
    void OnCollisionEnter(Collision other)
    {
        CheckNearItem(other.collider, true);
    }

    void OnCollisionExit(Collision other)
    {
        CheckNearItem(other.collider, false);
    }

    void OnTriggerEnter(Collider other)
    {
        CheckNearItem(other, true);
    }

    void OnTriggerExit(Collider other)
    {
        CheckNearItem(other, false);
    }

    void CheckNearItem(Collider collider, bool entering)
    {
        if (collider.CompareTag("Item"))
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance <= 2f)
            {
                if (entering)
                {
                    Debug.Log("Near Item");
                    isNearItem = true;
                    PickableItem pickableItem = collider.GetComponent<PickableItem>();
                    if (pickableItem != null)
                    {
                        ShowPickupText("Press F to pick up " + pickableItem.itemData.itemName);
                    }
                }
                else
                {
                    isNearItem = false;
                    pickupText.gameObject.SetActive(false);
                }
            }
        }
    }

    void HandleSlotSelectionInput()
    {
        // Check for key input to change the selected slot
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSlotIndex = 0;
            UpdateSelectionIndicator();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSlotIndex = 1;
            UpdateSelectionIndicator();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSlotIndex = 2;
            UpdateSelectionIndicator();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedSlotIndex = 3;
            UpdateSelectionIndicator();
        }
    }




    public void TryDropItem()
    {
        if (inventoryItems.ContainsKey(selectedSlotIndex))
        {
            PickableItem itemToDrop = inventoryItems[selectedSlotIndex];

            // Get the original prefab of the item
            GameObject originalPrefab = itemToDrop.itemData.itemPrefab;

            // Instantiate a new item based on the original prefab
            GameObject droppedItem = Instantiate(originalPrefab, transform.position + transform.forward * 1f, Quaternion.identity);

            // Set the position and activate the dropped item in the scene
            droppedItem.SetActive(true);

            // Remove the item from the inventory
            inventoryItems.Remove(selectedSlotIndex);

            // Destroy the instantiated inventory item prefab for the selected slot
            Destroy(invItemPrefabs[selectedSlotIndex]);

            // Clear the reference in the array
            invItemPrefabs[selectedSlotIndex] = null;

            // Apply a force to the dropped item to simulate it falling down
            Rigidbody droppedItemRb = droppedItem.GetComponent<Rigidbody>();
            if (droppedItemRb != null)
            {
                droppedItemRb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }
        }
    }



    public void TryPickupItem()
    {
        Collider[] colliders = new Collider[10]; // Adjust the size based on your needs
        int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, 2f, colliders);

        for (int i = 0; i < colliderCount; i++)
        {
            Collider collider = colliders[i];

            if (collider.CompareTag("Item") && pickupbool == true)
            {
                PickableItem pickableItem = collider.GetComponent<PickableItem>();
                if (pickableItem != null)
                {
                    // Check if the selected slot is already occupied
                    if (!inventoryItems.ContainsKey(selectedSlotIndex))
                    {
                        // Show pickup text
                        ShowPickupText("Picked up " + pickableItem.itemData.itemName);

                        // Pick up the item
                        PickUpItem(pickableItem);
                        pickupbool = false;
                    }
                    else
                    {
                        // Show a message indicating the slot is already occupied
                        ShowPickupText("Selected slot is already occupied. Drop the current item first.");
                    }
                    break;
                }
            }
        }
    }


    void PickUpItem(PickableItem item)
    {
        // Add the item to the inventory
        inventoryItems[selectedSlotIndex] = item;

        // Check if the slot already has an inventory prefab
        if (invItemPrefabs[selectedSlotIndex] != null)
        {
            // Destroy the existing inventory prefab for the selected slot
            Destroy(invItemPrefabs[selectedSlotIndex]);
        }

        // Instantiate the inventory item game object
        invItemPrefabs[selectedSlotIndex] = Instantiate(invItemPrefab, Vector3.zero, Quaternion.identity);
        invItemPrefabs[selectedSlotIndex].SetActive(true);

        // Set the parent of the instantiated object to the selected slot
        invItemPrefabs[selectedSlotIndex].transform.SetParent(inventorySlots[selectedSlotIndex].transform);

        // Set the position of the instantiated object to (0, 0, 0)
        invItemPrefabs[selectedSlotIndex].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // Set the scale of the instantiated object to (0.75, 0.75, 0.75)
        invItemPrefabs[selectedSlotIndex].transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

        // Set the sprite of the inventory item game object based on the ScriptableObject
        Image invItemImage = invItemPrefabs[selectedSlotIndex].GetComponent<Image>();
        invItemImage.sprite = item.itemData.itemIcon;

        // Deactivate the item in the scene
        item.gameObject.SetActive(false);

        // Show pickup text and hide it after 2 seconds (adjust the time as needed)
        ShowPickupText("Picked up item: " + item.itemData.itemName);
    }


    void UpdateSelectionIndicator()
    {
        selectionIndicator.transform.SetParent(inventorySlots[selectedSlotIndex].transform);
        selectionIndicator.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Debug.Log("Selected slot: " + (selectedSlotIndex + 1));
    }
    void ShowPickupText(string text, float displayTime = 2f)
    {
        // Display the pickup text with the provided message
        pickupText.text = text;
        pickupText.gameObject.SetActive(true);

        // Hide the pickup text after a delay
        StartCoroutine(HidePickupTextDelayed(displayTime));
    }

    IEnumerator HidePickupTextDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Hide the pickup text
        pickupText.gameObject.SetActive(false);
    }

}
