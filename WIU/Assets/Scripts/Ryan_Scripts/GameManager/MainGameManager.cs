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
        Vector3[] positionArray = new[] { 
            new Vector3(-16.6f, -9.65f, -8.47f)
        };

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
        Vector3[] positionArray = new[] {
            new Vector3(-26.433f, -8.326f, -7.981f),
            new Vector3(-22.89787f, -4.322f, -38.88957f),
            new Vector3(0.09983337f, -4.404f, 45.76365f),
            new Vector3(-0.5269532f, -0.607f, -44.68964f),
        };

        //loop through the array position
        for (int i = 0; i < positionArray.Length; ++i)
        {
            // Create a Chaser prefab
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Key"), positionArray[i], Quaternion.identity);

            obj.transform.parent = keys.transform;
        }
    }

    public void SpawnTank()
    {
        //Positions taken from the existing ghosts
        Vector3[] positionArray = new[] { 
            new Vector3(-13f, -5.8f, -8.1f),
            new Vector3(-12.9f, -1.55f, 15f)
        };

        //loop through the array position
        for (int i = 0; i < positionArray.Length; ++i)
        {
            // Create a Chaser prefab
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Tank"), positionArray[i], Quaternion.identity);

            obj.transform.parent = enemies.transform;
        }
    }

    public void SpawnMinion()
    {
        //Positions taken from the existing ghosts
        Vector3[] positionArray = new[] { 
            new Vector3(1f, -9.75f, -11.85f),
            new Vector3(-7.16f, -9.75f, -7.08f),
            new Vector3(-22.55f, -5.7f, -9.87f),
            new Vector3(-22.55f, -5.7f, 14.3f),
            new Vector3(-11.03f, -5.7f, 33.45f),
            new Vector3(-8.2f, -1.81f, 33.45f),
            new Vector3(-22.3f, -1.81f, 33.45f),
            new Vector3(-26.52f, -1.81f, 17.4f),
        };

        //loop through the array position
        for (int i = 0; i < positionArray.Length; ++i)
        {
            // Create a Chaser prefab
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Ghoul"), positionArray[i], Quaternion.identity);

            obj.transform.parent = enemies.transform;
        }
    }
}
