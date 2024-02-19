using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Sprite[] staminaSprites; // Array containing stamina sprites
    private Image staminaImage;
    private int maxStamina = 100;
    private int currentStamina;

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

    // Function to update stamina sprite based on current stamina value
    public void UpdateStaminaSprite(int stamina)
    {
        // Calculate the index of the sprite based on the fraction of stamina
        int spriteIndex = Mathf.Clamp(Mathf.FloorToInt((float)stamina / maxStamina * staminaSprites.Length), 0, staminaSprites.Length - 1);

        // Set the sprite based on the calculated index
        staminaImage.sprite = staminaSprites[spriteIndex];
    }

    // Function to decrease stamina
    private void DecreaseStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina - Mathf.RoundToInt(amount), 0, maxStamina);
        UpdateStaminaSprite(currentStamina);
    }

    // Function to refill stamina
    private void RefillStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + Mathf.RoundToInt(amount), 0, maxStamina);
        UpdateStaminaSprite(currentStamina);
    }
}
