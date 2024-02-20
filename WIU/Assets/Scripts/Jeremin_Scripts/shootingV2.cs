using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootingV2 : MonoBehaviour
{
    public float range = 100f; // Maximum range of the raycast
    public Camera playerCamera; // Reference to the player's camera
    public GameObject Fire;
    public GameObject HitPoint;

    void Update()
    {
        // Check for left mouse button click
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Play firing sound
        //firingSound.Play();

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
}
