using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private Light light;

    void Start()
    {

        light = GetComponent<Light>();
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            light.enabled = !light.enabled;
        }
    }
}
