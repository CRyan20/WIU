using System.Collections;
using UnityEngine;
using TMPro;

public class DoorInteraction : MonoBehaviour
{
    public float interactionDistance = 3f; // Distance to trigger interaction
    public KeyCode interactKey = KeyCode.F; // Key to trigger interaction
    public GameObject doorObject; // Reference to the door GameObject

    private bool isPlayerNearby = false;
    private bool isDoorOpened = false;
    private TextMeshProUGUI interactionText; // Reference to the UI text element

    void Start()
    {
        // Find the player object and get the TextMeshProUGUI component from it
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            interactionText = player.GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(interactKey))
        {
            if (!isDoorOpened)
                OpenDoor();
            else
                CloseDoor();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            UpdateInteractionText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactionText.gameObject.SetActive(false); // Hide interaction text when player moves away
        }
    }

    void UpdateInteractionText()
    {
        if (!isDoorOpened)
            interactionText.text = "Press F to Open";
        else
            interactionText.text = "Press F to Close";

        interactionText.gameObject.SetActive(true); // Show interaction text
    }

    void OpenDoor()
    {
        // Example: Rotate the door object around its Y-axis to simulate opening
        doorObject.transform.Rotate(Vector3.up, 89f);
        isDoorOpened = true;
        Debug.Log("The door is now open!");
        interactionText.gameObject.SetActive(false); // Hide interaction text after door is opened
    }

    void CloseDoor()
    {
        // Example: Rotate the door object around its Y-axis to simulate closing
        doorObject.transform.Rotate(Vector3.up, -89f);
        isDoorOpened = false;
        Debug.Log("The door is now closed!");
        interactionText.gameObject.SetActive(false); // Hide interaction text after door is closed
    }
}