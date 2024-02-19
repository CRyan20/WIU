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

            ////foreach (var wp in waypoints)
            ////{
            //    obj.GetComponent<ChaserAI>().waypoints.Add(wayPoints.transform.GetChild(i * 2));
            ////}
            ////assign the values
            //obj.GetComponent<ChaserAI>().waypoints.Add(wayPoints.transform.GetChild(i * 2));
            //obj.GetComponent<ChaserAI>().waypoints.Add(wayPoints.transform.GetChild(i * 2 + 1));
        }
    }
}
