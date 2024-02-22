using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooting : MonoBehaviour
{
    public float range = 100f; // Maximum range of the raycast
    public Camera playerCamera; // Reference to the player's camera
    public GameObject Fire;
    public GameObject HitPoint;
    //public AudioSource Firing;
    private bool canShoot = true; // Initialize to true to allow shooting at the start

    void Update()
    {
        // Check for left mouse button click and if the player can shoot
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            Shoot();
            // Start the firing rate coroutine
            StartCoroutine(FiringRate(0.7f)); // Adjust the firing rate here (e.g., 0.2f for 5 shots per second)
        }
    }

    void Shoot()
    {
        //Firing.Play();

        AudioManager.instance.PlaySound("Firing");

        // Create a ray from the camera's position forward
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawLine(playerCamera.transform.position, playerCamera.transform.position + playerCamera.transform.forward * range, Color.red, 0.1f);
        // Perform the raycast
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range))
        {
            // Check if the object hit has a HealthSystem component
            HealthSystem healthSystem = hit.collider.GetComponent<HealthSystem>();
            if (healthSystem != null)
            {
                // If it does, reduce its health
                healthSystem.TakeDamage(10); // You can adjust the damage value as needed
            }

            Fire.GetComponent<ParticleSystem>().Play();
            GameObject a = Instantiate(Fire, hit.point, Quaternion.identity);
            GameObject b = Instantiate(HitPoint, hit.point, Quaternion.identity);

            Destroy(a, 1);
            Destroy(b, 1);
        }
    }

    IEnumerator FiringRate(float delayBetweenShots)
    {
        // Prevent shooting during the firing rate delay
        canShoot = false;
        // Wait for the specified delay before allowing shooting again
        yield return new WaitForSeconds(delayBetweenShots);
        canShoot = true; // Allow shooting again after the delay
    }
}
