using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance;
    public GameObject wayPoints;
    public GameObject enemies;
    public GameObject keys;

    public void Awake()
    {
        Instance = this;
    }

    public void SpawnChaser()
    {
        //Positions taken from the existing ghosts
        Vector3[] positionArray = new[] { new Vector3(-5.3f, 0f, -3.1f) };

        //loop through the array position
        for (int i = 0; i < positionArray.Length; ++i)
        {
            // Create a Chaser prefab
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Chaser"), positionArray[i], Quaternion.identity);

            obj.transform.parent = enemies.transform;
            obj.GetComponent<ChaserAI>().SetWaypoints(wayPoints);
        }
    }

    public void SpawnKey()
    {
        Vector3[] positionArray = new[] { new Vector3(-10f, 0f, -3.1f) };

        //loop through the array position
        for (int i = 0; i < positionArray.Length; ++i)
        {
            // Create a Chaser prefab
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Key"), positionArray[i], Quaternion.identity);

            obj.transform.parent = keys.transform;
        }
    }
}
