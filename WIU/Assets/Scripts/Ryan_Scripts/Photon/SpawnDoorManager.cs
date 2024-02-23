using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDoorManager : MonoBehaviour
{
    public static SpawnDoorManager Instance;
    public int spawnIndex = 0;

    public SpawnPointsDoor[] spawnPoints;

    void Awake()
    {
        Instance = this;
        spawnPoints = GetComponentsInChildren<SpawnPointsDoor>();
    }

    public Transform GetSpawnPoints()
    {
        //return spawnPoints[Random.Range(0, spawnPoints.Length)].transform;

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points found for doors!");
            return null;
        }
        Transform spawnPoint = spawnPoints[spawnIndex].transform;
        spawnIndex = (spawnIndex + 1) % spawnPoints.Length; // Move to the next spawn point
        return spawnPoint;
    }
}