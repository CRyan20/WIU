using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public float interactionDistance = 3f; // Distance to trigger interaction
    public KeyCode interactKey = KeyCode.F; // Key to trigger interaction
    public GameObject doorObject; // Reference to the door GameObject

    private bool isPlayerNearby = false;
    private bool isDoorOpened = false;

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
            if (!isDoorOpened)
                Debug.Log("Press 'F' to open the door.");
            else
                Debug.Log("Press 'F' to close the door.");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    void OpenDoor()
    {
        // Example: Rotate the door object around its Y-axis to simulate opening
        doorObject.transform.Rotate(Vector3.up, 89f);
        isDoorOpened = true;
        Debug.Log("The door is now open!");
    }

    void CloseDoor()
    {
        // Example: Rotate the door object around its Y-axis to simulate closing
        doorObject.transform.Rotate(Vector3.up, -89f);
        isDoorOpened = false;
        Debug.Log("The door is now closed!");
    }
}
