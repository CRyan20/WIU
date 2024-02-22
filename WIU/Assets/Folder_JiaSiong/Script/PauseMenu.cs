using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
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
        if (photonView.IsMine)
        {
            // Check for the "Escape" key to toggle the pause menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenu();
            }
        }
    }

    void TogglePauseMenu()
    {
        // Toggle the visibility of the pause menu
        pauseMenuUI.SetActive(!pauseMenuUI.activeSelf);
    }
}
