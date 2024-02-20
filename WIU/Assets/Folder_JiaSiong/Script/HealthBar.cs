using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Sprite[] healthSprites; // Array containing health sprites
    private Image healthImage;
    private int maxHealth = 100;

    private void Start()
    {
        healthImage = GetComponent<Image>();
        // Assuming the initial health is full
        UpdateHealthSprite(maxHealth);
    }

    // Function to update health sprite based on current health value
    public void UpdateHealthSprite(int currentHealth)
    {
        // Calculate the index of the sprite based on the fraction of health
        int spriteIndex = Mathf.Clamp(Mathf.FloorToInt((float)currentHealth / maxHealth * healthSprites.Length), 0, healthSprites.Length - 1);

        // Set the sprite based on the calculated index
        healthImage.sprite = healthSprites[spriteIndex];
    }

    // Example of updating health sprite with a new health value
    private void Update()
    {
        // For demonstration purposes, decrease health over time
        int currentHealth = Mathf.Max(0, Mathf.FloorToInt(Time.time * 10) % (maxHealth + 1));
        UpdateHealthSprite(currentHealth);
    }
}
