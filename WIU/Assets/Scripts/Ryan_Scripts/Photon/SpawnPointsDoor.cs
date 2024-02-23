using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsDoor : MonoBehaviour
{
    [SerializeField] GameObject graphics;

    void Awake()
    {
        graphics.SetActive(false);
    }
}
