using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Sprite[] staminaSprites; // Array containing stamina sprites
    private Image staminaImage;
    public int maxStamina;
    public int currentStamina;

    // Adjust this speed based on how fast you want the stamina to deplete and refill
    public float depletionSpeed = 19f;
    public float refillSpeed = 18f;

    private void Start()
    {
        staminaImage = GetComponent<Image>();
        currentStamina = maxStamina;
        UpdateStaminaSprite(currentStamina);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // Decrease stamina while holding the Shift key
            DecreaseStamina(Time.deltaTime * depletionSpeed);
        }
        else
        {
            // Refill stamina when the Shift key is released
            RefillStamina(Time.deltaTime * refillSpeed);
        }
    }

    public void SetMaxStamina(int maxstamina)
    {
        maxStamina = maxstamina;
    }
    public void UpdateStaminaSprite(int stamina)
    {
        int spriteIndex = 4;

        // Check specific values (100, 75, 50, 25) and set the sprite index accordingly
        if (stamina == 100)
        {
            spriteIndex = 4;
        }
        else if (stamina <= 75 && stamina >= 50)
        {
            spriteIndex = 3;
        }
        else if (stamina <= 50 && stamina >= 25)
        {
            spriteIndex = 2;
        }
        else if (stamina <= 25 && stamina > 0)
        {
            spriteIndex = 1;
        }
        else if (stamina == 0)
        {
            spriteIndex = 0;
        }

        Debug.Log("Stamina: " + stamina + ", Sprite Index: " + spriteIndex);
        staminaImage.sprite = staminaSprites[spriteIndex];
    }

    // Function to decrease stamina
    public void DecreaseStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina - Mathf.RoundToInt(amount), 0, maxStamina);
        // Check if the current stamina is a multiple of 25
        if (currentStamina % 25 == 0)
        {
            UpdateStaminaSprite(currentStamina);
        }
    }

    // Function to refill stamina
    public void RefillStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + Mathf.RoundToInt(amount), 0, maxStamina);
        // Check if the current stamina is a multiple of 25
        if (currentStamina % 25 == 0)
        {
            UpdateStaminaSprite(currentStamina);
        }
    }
}
