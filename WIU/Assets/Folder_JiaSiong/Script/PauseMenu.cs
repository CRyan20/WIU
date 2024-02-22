using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class PauseMenu : MonoBehaviourPunCallbacks
{
    public static PauseMenu Instance;

    public GameObject pauseMenuUI;
    public bool isPaused = false;

    void Awake()
    {
        Instance = this;    
    }

    void Start()
    {
        // Ensure the pause menu is initially hidden
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        // Check for the "Escape" key to toggle the pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        isPaused = !isPaused;
        pauseMenuUI.SetActive(isPaused);
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
        // Notify other scripts about the pause state
        SendMessage("OnPause", isPaused, SendMessageOptions.DontRequireReceiver);
    }
}
