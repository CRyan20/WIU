using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class EndDoor : MonoBehaviour
{
    public static EndDoor Instance;

    public float interactionDistance = 3f; // Distance to trigger interaction
    public KeyCode interactKey = KeyCode.I; // Key to trigger interaction
    public GameObject doorObject; // Reference to the door GameObject

    public int keysCollected = 0; // Number of keys collected
    public int keysRequired = 4; // Number of keys required to open the door

    public bool isPlayerNearby = false;
    public bool isDoorOpened = false;

    public TextMeshProUGUI pickupText; // Reference to TextMeshProUGUI

    public GameObject GameWinScreen;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GameWinScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(interactKey))
        {
            if (!isDoorOpened && keysCollected >= keysRequired)
            {
                //end the game
                EndGame();
            }
            else
            {
                ShowPickupText("The door requires " + (keysRequired - keysCollected) + " more keys to open.");
                Debug.Log("The door requires " + (keysRequired - keysCollected) + " more keys to open.");
            }
        }  
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (!isDoorOpened)
                Debug.Log("Press 'I' to input key");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    public void CollectKey()
    {
        keysCollected++;
        Debug.Log("Key collected. Total keys: " + keysCollected);
    }

    public void EndGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
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
