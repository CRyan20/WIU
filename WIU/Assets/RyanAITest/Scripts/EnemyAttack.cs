using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // Reference to the player prefab
    public GameObject playerPrefab;

    // This method is called by the animation event
    public void DealDamageToPlayer(int damageAmount)
    {
        // Find the player in the scene
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        // Check if the player object exists
        if (playerObject != null)
        {
            // Get the HealthSystem component from the player object
            HealthSystem playerHealth = playerObject.GetComponent<HealthSystem>();

            // Check if player health system exists
            if (playerHealth != null)
            {
                // Deal damage to the player
                playerHealth.TakeDamage(damageAmount);

                // Get the HealthBar component from the child object under the player
                HealthBar healthBar = playerObject.GetComponentInChildren<HealthBar>();

                // Update the health bar with the new health value
                if (healthBar != null)
                {
                    healthBar.UpdateHealthSprite(playerHealth.GetCurrentHealth());
                }
                else
                {
                    Debug.LogError("HealthBar reference not found in the EnemyAttack script!");
                }
            }
            else
            {
                Debug.LogError("Player Health reference not found in the EnemyAttack script!");
            }
        }
        else
        {
            Debug.LogError("Player object not found in the scene!");
        }
    }
}
