using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    //public AudioSource hit;

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        //hit.Play();
        currentHealth -= amount;
        SetCurrenHealth(currentHealth);
    }

    public int SetCurrenHealth(int amount)
    {
        currentHealth = amount;
        return currentHealth;
    }
}
