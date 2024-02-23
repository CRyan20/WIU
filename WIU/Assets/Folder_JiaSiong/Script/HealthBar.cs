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

    private void Update()
    {
        // Check if the player health has changed and update the health sprite accordingly
        if (playerHealth.GetCurrentHealth() != lastHealth)
        {
            UpdateHealthSprite(playerHealth.GetCurrentHealth());
        }
    }

    // Function to update health sprite based on current health value
    public void UpdateHealthSprite(int currentHealth)
    {
        int maxHealth = playerHealth.GetMaxHealth();
        int spriteIndex = 4;

        // Check specific values and set the sprite index accordingly
        if (currentHealth == 100)
        {
            spriteIndex = 4;
        }
        else if (currentHealth <= 75 && currentHealth > 50)
        {
            spriteIndex = 3;
        }
        else if (currentHealth <= 50 && currentHealth > 25)
        {
            spriteIndex = 2;
        }
        else if (currentHealth <= 25 && currentHealth > 0)
        {
            spriteIndex = 1;
        }
        else if (currentHealth == 0)
        {
            spriteIndex = 0;
        }

        // Set the sprite based on the calculated index
        healthImage.sprite = healthSprites[spriteIndex];

        // Update the last health value
        lastHealth = currentHealth;
    }
}
