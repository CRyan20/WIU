using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Sprite[] healthSprites; // Array containing health sprites
    private Image healthImage;
    public EnemyAttack healthSystem;
    private int lastHealth;

    private void Start()
    {

        healthImage = GetComponent<Image>();

        // Get the initial health value
        lastHealth = healthSystem.playerHealth.currentHealth;

        // Update the health sprite based on the initial health value
        UpdateHealthSprite(lastHealth);
    }


    // Function to update health sprite based on current health value
    public void UpdateHealthSprite(int currentHealth)
    {
        int maxHealth = healthSystem.playerHealth.maxHealth;

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


    // Example of updating health sprite with a new health value
    private void Update()
    {
        // For demonstration purposes, decrease health over time
        UpdateHealthSprite(lastHealth);
        Debug.Log("test" + lastHealth);
    }
}
