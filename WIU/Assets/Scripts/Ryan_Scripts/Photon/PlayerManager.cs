using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        ////Positions taken from the existing ghosts
        //Vector3[] positionArray = new[] 
        //{ 
        //    new Vector3(-30f, -9.46f, 18.7f), //player 1
        //    new Vector3(-17f, -9.46f, 18.38f), //player 2
        //    new Vector3(-10.8f, -9.46f, 19.3f), //player 3
        //    new Vector3(0f, -9.46f, 19.3f) //player 4
        //};

        //// Get the number of players in the room
        //int numPlayersInRoom = PhotonNetwork.CurrentRoom.PlayerCount;

        //for (int i = 0; i < Mathf.Min(numPlayersInRoom, positionArray.Length); ++i)
        //{
        //PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), Vector3.zero, Quaternion.identity);
        //}

        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoints();
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawnPoint.position, spawnPoint.rotation);
    }
}
