using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    SpawnPoints[] spawnPoints;

    void Awake()
    {
        Instance = this;
        spawnPoints = GetComponentsInChildren<SpawnPoints>();
    }

    public Transform GetSpawnPoints()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
    }
}
