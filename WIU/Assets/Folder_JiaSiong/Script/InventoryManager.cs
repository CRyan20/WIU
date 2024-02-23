using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryCanvas;
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
    private PhotonView photonView;
    private PickableItem pickableItem;
    private GameObject nearestItem;

    public GameObject doorObject;
    private EndDoor endDoor;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        invItemPrefabs = new GameObject[inventorySlots.Length];

        if (!photonView.IsMine)
        {
            inventoryCanvas.SetActive(false);
        }

        doorObject = GameObject.FindGameObjectWithTag("Exit");
        if (doorObject != null)
        {
            // Get the EndDoor component from the door GameObject
            endDoor = doorObject.GetComponent<EndDoor>();

            if (endDoor == null)
            {
                Debug.LogError("EndDoor component not found on the door GameObject.");
            }
        }
        else
        {
            Debug.LogError("Door GameObject not found in the scene.");
        }
    }
    void Update()
    {
        if (!photonView.IsMine) return;
        HandleSlotSelectionInput();

        // Check for key input to pick up and drop items
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isNearItem) // Check if the player is near an item before picking up
            {
                pickupbool = true;
                // Store the viewID of the item
                PhotonView photonView = nearestItem.GetComponent<PhotonView>();
                int viewID = photonView.ViewID;
                TryPickupItem(viewID);

            }
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            TryDropItem();
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            TryUseKey();
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
        if (!photonView.IsMine) return;
        if (collider.CompareTag("Item"))
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance <= 2f)
            {
                if (entering)
                {
                    Debug.Log("Near Item");
                    nearestItem = collider.gameObject;
                    isNearItem = true;
                    PickableItem pickableItem = nearestItem.GetComponent<PickableItem>();
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

    public void TryUseKey()
    {
        // Check if the player has any keys in the inventory
        if (inventoryItems.ContainsKey(selectedSlotIndex))
        {
            // Check if the item in the selected slot is a key
            PickableItem itemInSlot = inventoryItems[selectedSlotIndex];
            if (itemInSlot.itemData.itemType == ItemType.Key)
            {
                // Increment the keys collected
                endDoor.keysCollected++;

                // Remove the used key from the inventory
                inventoryItems.Remove(selectedSlotIndex);

                // Destroy the instantiated inventory item prefab for the selected slot
                Destroy(invItemPrefabs[selectedSlotIndex]);

                // Clear the reference in the array
                invItemPrefabs[selectedSlotIndex] = null;


                // Destroy the instantiated inventory item prefab for the selected slot
                Destroy(inventorySlots[selectedSlotIndex].GetComponentsInChildren<MonoBehaviour>()[2].gameObject);
                //Destroy(itemInSlot.gameObject);

                // Update UI and provide feedback
                ShowPickupText("Used a key. Keys remaining: " + (endDoor.keysRequired - endDoor.keysCollected));

                // Check if enough keys have been collected to open the door
                if (endDoor.keysCollected == 4)
                {
                    // Open the door
                    endDoor.isDoorOpened = true;
                    // Perform any additional actions related to door opening

                    // End the game
                    EndDoor.Instance.EndGame();
                }
            }
            else
            {
                // The item in the selected slot is not a key
                ShowPickupText("Selected item is not a key.");
            }
        }
        else
        {
            // The player doesn't have any keys in the inventory
            ShowPickupText("No keys in inventory.");
        }
    }


    public void TryDropItem()
    {
        if (inventoryItems.ContainsKey(selectedSlotIndex))
        {
            photonView.RPC("DropItem", RpcTarget.AllBuffered, selectedSlotIndex);

            //PickableItem itemToDrop = inventoryItems[selectedSlotIndex];

            //// Get the original prefab of the item
            //GameObject originalPrefab = itemToDrop.itemData.itemPrefab;

            //// Instantiate a new item based on the original prefab
            //GameObject droppedItem = Instantiate(originalPrefab, transform.position + transform.forward * 1f, Quaternion.identity);

            //// Set the position and activate the dropped item in the scene
            //droppedItem.SetActive(true);

            //// Remove the item from the inventory
            //inventoryItems.Remove(selectedSlotIndex);

            //// Destroy the instantiated inventory item prefab for the selected slot
            //Destroy(invItemPrefabs[selectedSlotIndex]);

            //// Clear the reference in the array
            //invItemPrefabs[selectedSlotIndex] = null;

            //// Apply a force to the dropped item to simulate it falling down
            //Rigidbody droppedItemRb = droppedItem.GetComponent<Rigidbody>();
            //if (droppedItemRb != null)
            //{
            //    droppedItemRb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            //}
        }
    }

    [PunRPC]
    void DropItem(int slotIndex)
    {
        if (!photonView.IsMine) return;

        slotIndex = selectedSlotIndex;

        PickableItem itemToDrop = inventoryItems[slotIndex];


        // Get the original prefab of the item
        GameObject originalPrefab = itemToDrop.itemData.itemPrefab;
        //GameObject droppedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Key"), transform.position + transform.forward * 1f, Quaternion.identity);
        // Instantiate a new item based on the original prefab
        GameObject droppedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", originalPrefab.name), transform.position + transform.forward * 1f, Quaternion.identity);
        // Set the position and activate the dropped item in the scene
        droppedItem.SetActive(true);

        // Remove the item from the inventory
        inventoryItems.Remove(slotIndex);

        // Destroy the instantiated inventory item prefab for the selected slot
        Destroy(invItemPrefabs[slotIndex]);

        // Clear the reference in the array
        invItemPrefabs[slotIndex] = null;

        Debug.Log(invItemPrefab);

        // Destroy the instantiated inventory item prefab for the selected slot
        Destroy(inventorySlots[slotIndex].GetComponentsInChildren<MonoBehaviour>()[2].gameObject);

        //UpdateInventoryUI();

        // Clear the reference in the array
        //invItemPrefabs[slotIndex] = null;

        // Apply a force to the dropped item to simulate it falling down
        Rigidbody droppedItemRb = droppedItem.GetComponent<Rigidbody>();
        if (droppedItemRb != null)
        {
            droppedItemRb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
    }



    public void TryPickupItem(int viewID)
    {
        Collider[] colliders = new Collider[10]; // Adjust the size based on your needs
        int colliderCount = Physics.OverlapSphereNonAlloc(transform.position, 2f, colliders);

        for (int i = 0; i < colliderCount; i++)
        {
            Collider collider = colliders[i];

            if (collider.CompareTag("Item") && pickupbool == true)
            {
                PickableItem pickableItem = collider.GetComponent<PickableItem>();
                if (pickableItem != null && pickableItem.Owner == null)
                {
                    // Check if the selected slot is already occupied
                    if (!inventoryItems.ContainsKey(selectedSlotIndex))
                    {
                        // Show pickup text
                        ShowPickupText("Picked up " + pickableItem.itemData.itemName);

                        // Pick up the item
                        pickableItem.Owner = PhotonNetwork.LocalPlayer;
                        //photonView.RPC("PickUpItem", RpcTarget.MasterClient, viewID);

                        //PickUpItem(photonView.ViewID, viewID);
                        photonView.RPC("PickUpItem", RpcTarget.All, photonView.ViewID, viewID);
                        //PickUpItem(pickableItem);
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

    [PunRPC]
    void PickUpItem(int myId, int viewID)
    {
        Debug.Log(photonView.ViewID);
        // Find the item GameObject using the PhotonView ID
        GameObject itemGO = PhotonView.Find(viewID).gameObject;
        GameObject myGO = PhotonView.Find(myId).gameObject;
        PickableItem pickableItem = itemGO.GetComponent<PickableItem>();
        InventoryManager invent = myGO.GetComponent<InventoryManager>();
        Debug.Log(invent.photonView.ViewID);
        if (pickableItem != null)
        {
            // Get the ItemData from the PickableItem
            ItemData itemData = pickableItem.itemData;
            //PickableItem newItem = new PickableItem();
            //newItem.itemData = itemData;
            //newItem.Owner = PhotonNetwork.LocalPlayer;
            // Add the new PickableItem instance to the inventory
            inventoryItems[selectedSlotIndex] = pickableItem;
            
            // Instantiate the inventory item game object
            GameObject newItemGO = Instantiate(invItemPrefab, Vector3.zero, Quaternion.identity);
            newItemGO.SetActive(true);

            // Set the parent of the instantiated object to the selected slot
            newItemGO.transform.SetParent(inventorySlots[selectedSlotIndex].transform);

            // Set the position of the instantiated object to (0, 0, 0)
            newItemGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // Set the scale of the instantiated object to (0.75, 0.75, 0.75)
            newItemGO.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

            // Set the sprite of the inventory item game object based on the ItemData
            Image invItemImage = newItemGO.GetComponent<Image>();
            invItemImage.sprite = itemData.itemIcon;

            // Hide the pickup text
            pickupText.gameObject.SetActive(false);
            //newItemGO.transform.root.gameObject.SetActive(false);
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    // Call the RPC to deactivate the item on all clients
            //    photonView.RPC("DeactivateItem", RpcTarget.All, viewID);
            //}

            // Show pickup text and hide it after 2 seconds (adjust the time as needed)
            ShowPickupText("Picked up item: " + itemData.itemName);
        }
        //Destroy(inventorySlots[selectedSlotIndex].gameObject);
        Destroy(itemGO);

    }

    //[PunRPC]
    void DeactivateItem(GameObject go)
    {
        // Deactivate the item in the scene
        if(go!=null)
        go.SetActive(false);
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
