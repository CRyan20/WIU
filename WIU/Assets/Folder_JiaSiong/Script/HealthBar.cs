using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Sprite[] healthSprites; // Array containing health sprites
    private Image healthImage;
    private HealthSystem playerHealth;
    private int lastHealth;

    private void Start()
    {
        healthImage = GetComponent<Image>();

        if (healthImage == null)
        {
            Debug.LogError("HealthImage component not found on the HealthBar object!");
            return;
        }

        Debug.Log("HealthImage component successfully obtained.");

        // Find the player in the scene
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            Debug.Log("Player object found.");

            // Get the HealthSystem component from the player object
            playerHealth = playerObject.GetComponent<HealthSystem>();

            if (playerHealth != null)
            {
                Debug.Log("Player Health component found.");

                // Initialize lastHealth with player's current health
                lastHealth = playerHealth.GetCurrentHealth();

                // Update the health sprite based on the initial health value
                UpdateHealthSprite(lastHealth);
            }
            else
            {
                Debug.LogError("Player Health reference not found on the player object!");
            }
        }
        else
        {
            Debug.LogError("Player object not found in the scene!");
        }
    }


    // Function to update health sprite based on current health value
    public void UpdateHealthSprite(int currentHealth)
    {
        int maxHealth = playerHealth.GetMaxHealth();

        // Calculate the index of the sprite based on the fraction of health
        int spriteIndex = Mathf.Clamp(Mathf.FloorToInt((float)currentHealth / maxHealth * (healthSprites.Length - 1)), 0, healthSprites.Length - 1);

        // Set the sprite based on the calculated index
        healthImage.sprite = healthSprites[spriteIndex];

        // Check if the health has changed by at least 25 HP
        if (Mathf.Abs(currentHealth - lastHealth) >= 25)
        {
            // Update the last health value and perform any additional actions
            lastHealth = currentHealth;
            // Add any additional actions here
        }
    }
}
