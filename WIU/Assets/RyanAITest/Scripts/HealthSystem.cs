using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 20;
    public int currentHealth;
    public AudioSource hit;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        hit.Play();
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }

    }

    void Die()
    {
        // Perform any necessary actions before destroying the game object, such as playing death animations, spawning effects, etc.
        Destroy(gameObject);
    }
}

