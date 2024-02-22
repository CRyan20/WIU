using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // Reference to the player's health system
    public HealthSystem playerHealth;

    private void Start()
    {
        playerHealth.currentHealth = playerHealth.maxHealth;
    }

    private void Update()
    {
        if (playerHealth.currentHealth <= 0)
        {
            playerHealth.currentHealth = 0;
        }
        playerHealth.SetCurrenHealth(playerHealth.currentHealth);
    }

    // This method is called by the animation event
    public void DealDamageToPlayer(int damageAmount)
    {

        // Check if player health system exists
        if (playerHealth != null)
        {
            // Deal damage to the player
            playerHealth.TakeDamage(damageAmount);
            playerHealth.SetCurrenHealth(playerHealth.currentHealth);
            //Debug.Log(playerHealth.currentHealth);
        }
    }
}
