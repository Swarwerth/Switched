using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    private void Start()
    {

        if (PhotonNetwork.IsMasterClient) PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[0].position, Quaternion.identity);
        else PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[0].position, Quaternion.identity);
    }
}
