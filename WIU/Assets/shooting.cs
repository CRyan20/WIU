using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooting : MonoBehaviour
{
    public Transform FirePoint;
    public GameObject Fire;
    public GameObject HitPoint;
    public AudioSource firing;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shooting();
            firing.Play();
        }
    }

    public void Shooting()
    {
        RaycastHit hit;

        if (Physics.Raycast(FirePoint.position, transform.TransformDirection(Vector3.forward), out hit, 100))
        {
            Debug.DrawRay(FirePoint.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);

            Fire.GetComponent<ParticleSystem>().Play();
            GameObject a = Instantiate(Fire, FirePoint.position, Quaternion.identity);
            GameObject b = Instantiate(HitPoint, hit.point, Quaternion.identity);

            Destroy(a, 1);
            Destroy(b, 1);
            //enemy enemy = hit.transform.GetComponent<enemy>();
            //if (enemy != null)
            //{
            //    enemy.TakeDamage(5);
            //}

        }
    }
}
